// INNAHEMA
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using BLToolkit.Reflection.Emit;
using JetBrains.Annotations;

namespace SUF.Common.GeneralPurpose
{
    /// <summary>
    /// Как и не типизированый делегат содержит метод DynamicInvoke и может содержать в себе много делегатов но при этом не мешает работать GC
    /// </summary>
    public abstract class WeakDelegate
    {
        /// <summary>
        /// Производит вызов всех делегатов внутри.
        /// Проводит проверку типов параметров.
        /// </summary>
        /// <exception cref="TargetParameterCountException">Размерность масива не совпадает с количеством параметров  делегата</exception>
        /// <exception cref="ArgumentException">Тип одного из  параметров не соответсвует сигнатуре</exception>
        /// <exception cref="ArgumentNullException">Попытка передать значение null в тип данных по значению aka. struct</exception>
        public abstract object DynamicInvoke(params object[] parms);

        internal WeakDelegate(){}
    }

    /// <summary>
    /// Как и делегат содержит метод Invoke и может содержать в себе много делегатов но при этом не мешает работать GC
    /// </summary>
    public sealed class WeakDelegate<TDelegate> : WeakDelegate
        where TDelegate:class 
    {
        static WeakDelegate()
        {
            if (!(typeof(TDelegate).GetParents().Contains(typeof(MulticastDelegate))))
                throw ExceptionHelper.Throw<NotSupportedException>(
                    "Параметер типа для {0} должен быть делегат а не {1}",
                    "WeakDelegate<TDelegate>", typeof (TDelegate).Name);

            var signature = typeof(TDelegate).GetMethod("Invoke");
            paramSig = signature.GetParameters().ToArray();

            if(paramSig.Length!=0 && paramSig.FirstOrDefault(p=>p.IsOut|p.IsRetval)!=null)
                throw ExceptionHelper.Throw<NotSupportedException>(
                    "Делегаты с типом праметров Ret/Out пока не потдерживаются, если надо обращайтесь к winnie");

            var module = DynamicAssemblyProvider.GetModule("WeakDelegation");

            var typeBuilder = module.DefineType("Invokator_"+typeof(TDelegate),TypeAttributes.Sealed,typeof(Invokator));

            var constructor 
                = typeBuilder.DefineConstructor(MethodAttributes.Private,CallingConventions.Any,new []{typeof(WeakDelegate<TDelegate>)});



            new EmitHelper(constructor.GetILGenerator())
                .ldarg_0
                .ldarg_1
                .call(typeof(Invokator).GetConstructors((BindingFlags)(-1)).First())
                .ret();

            var met 
                = typeBuilder.DefineMethod("Invoke",MethodAttributes.Public,CallingConventions.HasThis,signature.ReturnType,paramSig.Select(p=>p.ParameterType).ToArray());
             
            var met_helper = new EmitHelper(met.GetILGenerator());

            met_helper = met_helper
                .ldarg_0
                .ldc_i4_(paramSig.Length)
                .newarr(typeof (object));

            for (int i = 1; i <= paramSig.Length; i++)
            {
                met_helper = met_helper
                    .dup
                    .ldc_i4_(i - 1)
                    .ldarg(i)
                    .boxIfValueType(paramSig[i-1].ParameterType)
                    .stelem_ref;
            }
            

            met_helper
                .call(typeof (Invokator).GetMethod("DynamicInvoke",(BindingFlags)(-1)));
            if (signature.ReturnType != typeof (void))
                met_helper
                    .unboxIfValueType(signature.ReturnType)
                    .ret();
            else
                met_helper
                    .pop
                    .ret();

            new EmitHelper(
                typeBuilder.DefineMethod("Instantiate", MethodAttributes.Static, typeof(Invokator), new[] { typeof(WeakDelegate<TDelegate>) })
                    .GetILGenerator())

                .ldarg_0
                .newobj(constructor)
                .ret();

            var invokatorType = typeBuilder.CreateType();

            invokator_creator =
                (Func<WeakDelegate<TDelegate>, Invokator>)
                Delegate.CreateDelegate(typeof (Func<WeakDelegate<TDelegate>, Invokator>),
                                        invokatorType.GetMethod("Instantiate", (BindingFlags) (-1)));

            invMethod = invokatorType.GetMethod("Invoke");
        }

        /// <summary>Удаляет делегат из списка вызовов</summary>
        /// <returns>Если такой делегат найден внутри и успешно удален</returns>
        public bool Remove(TDelegate @delegate)
        {
            int c = 0;
            if (@delegate != null)
                lock (dels)
                    foreach (var del in ((Delegate)(object)@delegate).GetInvocationList())
                        if (del.Method.IsStatic)
                            _invk = Delegate.Remove(_invk, del);
                        else
                            c += dels.RemoveAll(d => d.e1.Target == del.Target & d.e2 == del.Method.GetInvokator());

            return c != 0;
        }

        public bool Contains(TDelegate @delegate)
        {
            if (@delegate != null)
                lock (dels)
                    foreach (var del in ((Delegate)(object)@delegate).GetInvocationList())
                        if (del.Method.IsStatic)
                            return _invk.GetInvocationList().Contains(del);
                        else
                            if (!dels.Where(d => d.e1.Target == del.Target & d.e2 == del.Method.GetInvokator()).IsEmpty())
                                return true;

            return false;
        }


        public int Count
        {
            get
            {
                Clean();
                return dels.Count();
            }
        }

        private void Clean()
        {
            var toDel = new HashSet<Tuple<WeakReference, Func<object, object[], object>>>();
            lock (dels)
            {
                foreach (var del in dels)
                    if (!del.e1.IsAlive)
                        toDel.Add(del);

                dels.RemoveAll(toDel.Contains);
            }
        }

        /// <summary></summary>
        public void Add(TDelegate @delegate)
        {
            if (@delegate != null)
                lock (dels)
                    foreach (var del in ((Delegate)(object)@delegate).GetInvocationList())
                        if(del.Method.IsStatic)
                            _invk = Delegate.Combine(_invk , del);
                        else
                            dels.Add(new Tuple<WeakReference, Func<object, object[], object>>(new WeakReference(del.Target), del.Method.GetInvokator()));
        }

        /// <summary>Копирует все списки вызова из целевого делегата</summary>
        public void Add(WeakDelegate<TDelegate> other)
        {
            if (other != null)
                lock (dels)
                {
                    lock (other.dels)
                        dels.AddRange(other.dels);

                    foreach (var del in other._invk.GetInvocationList())
                        if (!del.Equals(other.Invoke))
                            _invk = Delegate.Combine(_invk, del);
                }
        }

        #region Constructors

        /// <summary>Создает на основе сусчествующего делегата</summary>
        public WeakDelegate(TDelegate target)
            : this()
        {
            Add(target);
        }

        /// <summary>Копирует все списки вызова из целевого делегата</summary>
        public WeakDelegate(WeakDelegate<TDelegate> other)
            : this()
        {
            Add(other);
        }

        /// <summary>Создает пустой "делегат", при попытке вызова ничег оне происходит!</summary>
        public WeakDelegate()
        {
            invokator = invokator_creator(this);
            _invoke = (TDelegate)(object)MulticastDelegate.CreateDelegate(typeof(TDelegate), invokator, invMethod);
        } 
        #endregion

        #region Operators

        public static WeakDelegate<TDelegate> operator +(WeakDelegate<TDelegate> wd, TDelegate d)
        {
            var D = new WeakDelegate<TDelegate>(wd);
            D.Add(d);
            return D;
        }

        public static WeakDelegate<TDelegate> operator -(WeakDelegate<TDelegate> wd, TDelegate d)
        {
            var D = new WeakDelegate<TDelegate>(wd);
            D.Remove(d);
            return D;
        }


        #endregion

        Invokator invokator;
        private static readonly MethodInfo invMethod;

        private List<Tuple<WeakReference, Func<object, object[], object>>> dels = new List<Tuple<WeakReference, Func<object, object[], object>>>();

        private TDelegate _invoke;
        private Delegate _invk
        {
            get
            {
                return (Delegate)(object)_invoke;
            }
            set
            {
                _invoke = (TDelegate)(object)value;
            }
        }
        private static Func<WeakDelegate<TDelegate>,Invokator> invokator_creator;
        private static ParameterInfo[] paramSig;

        /// <summary>Вызов делегата!</summary>
        public TDelegate Invoke
        {
            [DebuggerStepThrough]
            get
            {
                return _invoke;
            }
        }

        /// <summary>
        /// Производит вызов всех делегатов внутри.
        /// Проводит проверку типов параметров.
        /// </summary>
        /// <exception cref="TargetParameterCountException">Размерность масива не совпадает с количеством параметров  делегата</exception>
        /// <exception cref="ArgumentException">Тип одного из  параметров не соответсвует сигнатуре</exception>
        /// <exception cref="ArgumentNullException">Попытка передать значение null в тип данных по значению aka. struct</exception>
        public override object DynamicInvoke(params object[] parms)
        {
            CheckParameters(parms);
            return _dynamicInvoke(parms);
        }

        private void CheckParameters(object[] parms)
        {
            if (parms.Length != paramSig.Length)
                throw ExceptionHelper.Throw<TargetParameterCountException>();

            for (int i = 0; i < parms.Length; i++)
                if (parms[i] != null)
                {
                    if (!paramSig[i].ParameterType.IsInstanceOfType(parms[i]))
                        throw ExceptionHelper.Throw<ArgumentException>("В параметра {0} неправильный тип", i);
                }
                else if (paramSig[i].ParameterType.IsValueType)
                    throw ExceptionHelper.Throw<ArgumentNullException>(
                        "В пааметра {0} недопустимое значение для не ссылочного типа", i);
        }

        private object _dynamicInvoke(params object[] parms)
        {
            var toDel = new List<Tuple<WeakReference, Func<object, object[], object>>>();
            object ret = null;
            lock (dels)
            {
                foreach (var del in dels)
                {
                    var target = del.e1.Target;
                    var method = del.e2;

                    if ( target != null)
                        ret = method.Invoke(target, parms);
                    else
                    {
                        toDel.Add(del);
                    }
                }
                dels.RemoveAll(toDel.Contains);
            }
            return ret;
        }
    
        /// <summary> Используется во внутренних целях для вызовов делегатов </summary>
        public abstract class Invokator
        {
            private readonly WeakDelegate<TDelegate> del;

            protected Invokator(WeakDelegate<TDelegate> @delegate)
            {
                del = @delegate;
            }

            protected object DynamicInvoke(params object[] parms)
            {
                return del != null ? del._dynamicInvoke(parms) : null;
            }
        }
    }
}
