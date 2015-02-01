using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using LambdaMicrobenchmarking;
using NUnit.Framework;
using SUF.Common.GeneralPurpose;

namespace TestProject
{
    [TestFixture]
    public class Benchmarks
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void StaticMethod() { }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void InstanceMethod() { }

        [Test]
        public void InvokatorPerformance()
        {
            const int iterCount = 1000;

            var type = GetType();
            var staticMethod = type.GetMethod("StaticMethod");
            var instanceMethod = type.GetMethod("InstanceMethod");

            for (int i = 0; i < iterCount; i++)
            {   // JITC warmup
                StaticMethod();
                InstanceMethod();
            }

            Script<int>.Of(new[]
            {
                new Tuple<String, Func<int>>("Reflectional static Invocation", () =>
                {
                    for (int i = 0; i < iterCount; i++)
                        staticMethod.Invoke(null, new object[0]);
                    return 1;
                }),
                new Tuple<String, Func<int>>("Reflectional inst   Invocation", () =>
                {
                    for (int i = 0; i < iterCount; i++)
                        instanceMethod.Invoke(this, new object[0]);
                    return 1;
                }),
                new Tuple<String, Func<int>>("Reflectional static Invocation(c)", () =>
                {
                    var parameters = new object[0];
                    for (int i = 0; i < iterCount; i++)
                    {
                        staticMethod.Invoke(null, parameters);
                    }
                    return 1;
                }),
                new Tuple<String, Func<int>>("Reflectional inst   Invocation(c)", () =>
                {
                    var parameters = new object[0];
                    for (int i = 0; i < iterCount; i++)
                    {
                        instanceMethod.Invoke(this, parameters);
                    }
                    return 1;
                }),
                new Tuple<String, Func<int>>("Invocator static Invocation", () =>
                {
                    var parameters = new object[0];
                    for (int i = 0; i < iterCount; i++)
                        staticMethod.GetInvokator().Invoke(null, parameters);
                    return 1;
                }),
                new Tuple<String, Func<int>>("Invocator static Invocation(c)", () =>
                {
                    var invokation = staticMethod.GetInvokator();
                    var parameters = new object[0];
                    for (int i = 0; i < iterCount; i++)
                        invokation.Invoke(null, parameters);
                    return 1;
                }),
                new Tuple<String, Func<int>>("Invocator inst  Invocation(c)", () =>
                {
                    var invokation = instanceMethod.GetInvokator();
                    var parameters = new object[0];
                    for (int i = 0; i < iterCount; i++)
                    {
                        invokation.Invoke(this, parameters);
                    }
                    return 1;
                }),
                new Tuple<String, Func<int>>("Invocator inst  Invocation", () =>
                {
                    var parameters = new object[0];
                    for (int i = 0; i < iterCount; i++)
                        instanceMethod.GetInvokator().Invoke(this, parameters);
                    return 1;
                }),
            }).WithHead().RunAll();
        }

        [Test]
        public void DelegatePerformance()
        {
            const int iterCount = 1000;

            Action act = InstanceMethod;
            WeakDelegate<Action> wd = new WeakDelegate<Action>(InstanceMethod);

            for (int i = 0; i < iterCount; i++)
            {   // JITC warmup
                act.Invoke();
                wd.Invoke.Invoke();
            }


            Script<int>.Of(new[]
            {
                new Tuple<String, Func<int>>("Action", () =>
                {
                    for (int i = 0; i < iterCount; i++)
                        act.Invoke();
                    return 1;
                }),
                new Tuple<String, Func<int>>("WeakDelegate<Action>", () =>
                {
                    for (int i = 0; i < iterCount; i++)
                        wd.Invoke();
                    return 1;
                }),
            }).WithHead().RunAll();
        }
    }
}
