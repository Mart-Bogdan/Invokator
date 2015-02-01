using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace SUF.Common.GeneralPurpose
{
    public delegate object Invokation(object target, params object[] args);

    public static class InvokatorFactory
    {
        static readonly Dictionary<System.Tuple<MethodInfo, bool>, Invokation> cache = new Dictionary<System.Tuple<MethodInfo, bool>, Invokation>();
        static Int64 count = 0;
        private static readonly Type[] _args = new[] { typeof(object), typeof(object[]) };

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
            var method = new DynamicMethod("_" + count++, typeof(object), _args,
                                               typeof(InvokatorFactory), true);

            var generator = method.GetILGenerator();
            var parameters = methodInfo.GetParameters();
            var len = parameters.Length;
                //--------------------
            if (!methodInfo.IsStatic)
            {
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Castclass, methodInfo.DeclaringType);
            }
            //--------------------
            for (int i = 0; i < len; i++)
            {
                var type = parameters[i].ParameterType;
                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Ldc_I4_S, i);
                generator.Emit(OpCodes.Ldelem_Ref);

                generator.Emit(!type.IsValueType ? OpCodes.Castclass : OpCodes.Unbox_Any, type);
            }
            //--------------------
            generator.Emit( invokeVirtual ? OpCodes.Callvirt : OpCodes.Call, methodInfo);

            var returnType = methodInfo.ReturnType;

            if (returnType == typeof(void))
                generator.Emit(OpCodes.Ldnull);
            else
            {
                if (returnType.IsValueType)
                    generator.Emit(OpCodes.Box, returnType);
            }
            generator.Emit(OpCodes.Ret);

            var fun = (Invokation)method.CreateDelegate(typeof(Invokation));
            return fun;
        }
    }
}