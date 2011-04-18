 using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using BLToolkit.Reflection.Emit;

namespace SUF.Common.GeneralPurpose
{
    public delegate object Invokation(object target, params object[] args);

    public static class InvokatorFactory
    {
        static Dictionary<MethodInfo, Invokation> cache = new Dictionary<MethodInfo, Invokation>();
        static Random r = new Random();
        private static Type[] _args = new[] { typeof(object), typeof(object[]) };

        public static Invokation GetInvokator(this MethodInfo mi)
        {
            return GetInvokator(mi, true);
        }

        public static Invokation GetInvokator(this MethodInfo mi, bool invokeVirtual)
        {
            lock (cache)
            {
                Invokation fun;
                if (cache.TryGetValue(mi, out fun))
                    return fun;
                
                var method = new DynamicMethod("_" + r.Next(), mi.DeclaringType ?? typeof(object), _args,
                                               typeof (InvokatorFactory), true);

                var generator = method.GetILGenerator();
                var helper = new EmitHelper(generator);
                var parameters = mi.GetParameters();
                var len = parameters.Length;
                //--------------------
                if (!mi.IsStatic)
                    helper
                        .ldarg_0
                        .castclass(mi.DeclaringType);
                //--------------------
                for (int i = 0; i < len; i++)
                    helper
                        .ldarg_1
                        .ldc_i4_(i)
                        .ldelem_ref
                        .CastFromObject(parameters[i].ParameterType);
                //--------------------
                if (invokeVirtual)
                    helper.callvirt(mi);
                else
                    helper.call(mi);

                helper
                    .parseRet(mi.ReturnType)
                    .ret();

                fun = (Invokation)method.CreateDelegate(typeof(Invokation));
                cache.Add (mi, fun);

                return fun;
            }
        }

        private static EmitHelper parseRet(this EmitHelper hlpr, Type t)
        {
            if (t.Equals(typeof(void)))
                return hlpr.ldnull;
            else
                return hlpr.boxIfValueType(t);
        }
    }
}
