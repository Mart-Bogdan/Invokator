using System;
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

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestVoidMethod()
        {
            var o = GetType().GetMethod("SomeVoidMet").GetInvokator()(this,new object[]{10,"sdf"});
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
    }
}
