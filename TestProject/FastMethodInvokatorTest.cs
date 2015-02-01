using System;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SUF.Common.GeneralPurpose;

namespace TestProject
{
    [TestFixture]
    public class FastMethodInvokatorTest
    {
        [TestCase()]
        public void TestVoidMethod()
        {
            var o = GetType().GetMethod("SomeVoidMet").GetInvokator()(this, new object[] { 10, "sdf" });
            Assert.AreEqual(null, o);
        }

        [TestCase]
        public void TestIntMethod()
        {
            var o = GetType().GetMethod("SomeIntMet").GetInvokator()(this, new object[] { 10, "sdf" });
            Assert.AreEqual(10, o);
        }

        [TestCase]
        public void TestStrMethod()
        {
            var o = GetType().GetMethod("SomeStrMet").GetInvokator()(this, new object[] { 10, "sdf" });
            Assert.AreEqual("sdf", o);
        }

        [TestCase]
        public void TestStatMethod()
        {
            var o = GetType().GetMethod("SomeStatMet").GetInvokator()(null, new object[] { 10, "sdf" });
            Assert.AreEqual("sdf", o);
        }

        [TestCase]
        public void TestInvocatorsCache()
        {
            MethodInfo methodInfo = GetType().GetMethod("SomeStatMet");

            var orig = methodInfo.GetInvokator();
            var origTrue = methodInfo.GetInvokator(true);
            var origFalse = methodInfo.GetInvokator(false);
            var second = methodInfo.GetInvokator();
            var secondTrue = methodInfo.GetInvokator(true);

            Assert.AreSame(orig, second);
            Assert.AreNotSame(orig, origFalse, "Incorrect override");
            Assert.AreSame(orig, origTrue, "Incorrect override");
            Assert.AreSame(origTrue, secondTrue);
        }

        [TestCase]
        public void TestOverrideIgnoring()
        {
            var obj_ToString = typeof(Object).GetMethod("ToString");
            var inh_ToString = GetType().GetMethod("ToString");

            Assert.AreEqual(
                ToString(),
                obj_ToString.GetInvokator().Invoke(this));
            Assert.AreEqual(
                ToString(),
                inh_ToString.GetInvokator().Invoke(this));
            Assert.AreEqual(
                obj_ToString.GetInvokator().Invoke(this),
                inh_ToString.GetInvokator().Invoke(this));
            Assert.AreEqual(
                base.ToString(),
                obj_ToString.GetInvokator(false).Invoke(this));
            Assert.AreNotEqual(
                obj_ToString.GetInvokator(false).Invoke(this),
                inh_ToString.GetInvokator().Invoke(this));
        }

        #region MethodsToCall

        public void SomeVoidMet(int i, string ss)
        {
        }

        public int SomeIntMet(int i, string ss)
        {
            return i;
        }

        public string SomeStrMet(int i, string ss)
        {
            return ss;
        }

        public static string SomeStatMet(int i, string ss)
        {
            return ss;
        }

        public override string ToString()
        {
            return "ToString";
        }

        #endregion MethodsToCall
    }
}