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
        static Dictionary<System.Tuple<MethodInfo, bool>, Invokation> cache = new Dictionary<System.Tuple<MethodInfo, bool>, Invokation>();
        static Random r = new Random();
        private static Type[] _args = new[] { typeof(object), typeof(object[]) };

        public static Invokation GetInvokator(this MethodInfo methodInfo)
        {
            return GetInvokator(methodInfo, true);
        }

        public static Invokation GetInvokator(this MethodInfo methodInfo, bool invokeVirtual)
        {
            lock (cache)
            {
                Invokation fun;
                var methodKey = new System.Tuple<MethodInfo, bool>(methodInfo, invokeVirtual);
                if (!cache.TryGetValue(methodKey, out fun))
                {
                    fun = BuildInvokator(methodInfo, invokeVirtual);
                    cache.Add(methodKey, fun);
                }

                    return fun;
            }
        }

        private static Invokation BuildInvokator(MethodInfo methodInfo, bool invokeVirtual)
        {
            Invokation fun;
            var method = new DynamicMethod("_" + r.Next(), methodInfo.DeclaringType ?? typeof(object), _args,
                                               typeof(InvokatorFactory), true);

                var generator = method.GetILGenerator();
                var helper = new EmitHelper(generator);
            var parameters = methodInfo.GetParameters();
                var len = parameters.Length;
                //--------------------
            if (!methodInfo.IsStatic)
                    helper
                        .ldarg_0
                    .castclass(methodInfo.DeclaringType);
                //--------------------
                for (int i = 0; i < len; i++)
                    helper
                        .ldarg_1
                        .ldc_i4_(i)
                        .ldelem_ref
                        .CastFromObject(parameters[i].ParameterType);
                //--------------------
                if (invokeVirtual)
                helper.callvirt(methodInfo);
                else
                helper.call(methodInfo);

                helper
                .parseRet(methodInfo.ReturnType)
                    .ret();

                fun = (Invokation)method.CreateDelegate(typeof(Invokation));
                return fun;
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