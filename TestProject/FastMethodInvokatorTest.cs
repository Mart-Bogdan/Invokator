using System;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SUF.Common.GeneralPurpose;

namespace TestProject
{
    /// <summary>
    /// Summary description for FastMethodInvokatorTest
    /// </summary>
    [TestClass]
    public class FastMethodInvokatorTest
    {
        public FastMethodInvokatorTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void TestVoidMethod()
        {
            var o = GetType().GetMethod("SomeVoidMet").GetInvokator()(this, new object[] { 10, "sdf" });
        }

        [TestMethod]
        public void TestIntMethod()
        {
            var o = GetType().GetMethod("SomeIntMet").GetInvokator()(this, new object[] { 10, "sdf" });
        }

        [TestMethod]
        public void TestStrMethod()
        {
            var o = GetType().GetMethod("SomeStrMet").GetInvokator()(this, new object[] { 10, "sdf" });
        }

        [TestMethod]
        public void TestStatMethod()
        {
            var o = GetType().GetMethod("SomeStatMet").GetInvokator()(this, new object[] { 10, "sdf" });
        }

        [TestMethod]
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

        [TestMethod]
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

        public string SomeStatMet(int i, string ss)
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