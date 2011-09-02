using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;

namespace SUF.Common.GeneralPurpose
{
    public static class DynamicAssemblyProvider
    {
        private static AssemblyBuilder _assembly;

        public static AssemblyBuilder Assembly
        {
            get
            {
                if (_assembly != null)
                    return _assembly;

                lock (typeof(DynamicAssemblyProvider))
                {
                    if (_assembly != null)
                        return _assembly;

                    return
                        _assembly =
                        AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("GenPurpDynamicAssembly"),
                                                                      AssemblyBuilderAccess.RunAndSave);//,"C:\\tmp");
                }
            }
        }

        public static ModuleBuilder GetModule(String name)
        {
            lock (modules)
            {
                if (modules.ContainsKey(name))
                    return modules[name];

                return modules[name] = Assembly.DefineDynamicModule(name);
            }
        }

        private static Dictionary<string, ModuleBuilder> modules = new Dictionary<string, ModuleBuilder>();
    }
}