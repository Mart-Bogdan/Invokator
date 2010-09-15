 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using BLToolkit.Reflection.Emit;

namespace SUF.Common.GeneralPurpose
{
    public static class InvokatorFactory
    {
        static Dictionary<MethodInfo, Func<object, object[], object>> cache = new Dictionary<MethodInfo, Func<object, object[], object>>();
        static Random r = new Random();
        private static Type[] _args = new[] { typeof(object), typeof(object[]) };

        public static Func<object, object[], object> GetInvokator(this MethodInfo mi)
        {
            lock (cache)
            {
                Func<object, object[], object> fun;
                if (cache.TryGetValue(mi, out fun))
                    return fun;
                
                var method = new DynamicMethod("_" + r.Next(), typeof(object), _args,
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
                helper
                    .call(mi)
                    .parseRet(mi.ReturnType)
                    .ret();
                

                fun = (Func<object, object[], object>) method.CreateDelegate(typeof (Func<object, object[], object>));
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
