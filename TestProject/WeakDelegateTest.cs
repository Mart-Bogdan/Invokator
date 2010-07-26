using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SUF.Common.GeneralPurpose;

namespace TestProject
{
    /// <summary>
    /// Summary description for WeakDelegateTest
    /// </summary>
    [TestClass]
    public class WeakDelegateTest
    {
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
        public void TestMethod1()
        {
            try
            {
                var @delegate = new WeakDelegate<Func<String, int, String>>(Sm);
                var invoke = @delegate.Invoke("sd", 1);
                Assert.AreNotEqual(invoke, "sd");
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        [TestMethod]
        public void TestMethod2()
        {
            try
            {
                var @delegate = new WeakDelegate<Func<String, String>>(Sm);
                var invoke = @delegate.Invoke("sd");
                Assert.AreNotEqual(invoke, "sd");
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        [TestMethod]
        public void TestMethod3()
        {
            try
            {
                var @delegate = new WeakDelegate<Func<String, int, String>>(Sm);
                var invoke = @delegate.DynamicInvoke("sd", 5);
                Assert.AreNotEqual(invoke, "sd");
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public String Sm(String s)
        {
            return s;
        }
        public String Sm(String s, int o)
        {
            return s;
        }
    }
}
