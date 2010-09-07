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
        public void SRet_2p()
        {
            try
            {
                var @delegate = new WeakDelegate<Func<String, int, String>>(Sm);
                var invoke = @delegate.Invoke("sd", 1);
                Assert.AreEqual(invoke, "sd");
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [TestMethod]
        public void Unsubscribe()
        {
            try
            {
                var @delegate = new WeakDelegate<Func<String, int, String>>(Sm);
                @delegate -= Sm;
                var invoke = @delegate.Invoke("sd", 1);
                Assert.AreEqual(null, invoke);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [TestMethod]
        public void UnsubscribeStatic()
        {
            try
            {
                var @delegate = new WeakDelegate<Func<String,String>>(sSm);
                @delegate -= sSm;
                var invoke = @delegate.Invoke("sd");
                Assert.AreEqual(null, invoke);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [TestMethod]
        public void SRet()
        {
            try
            {
                var @delegate = new WeakDelegate<Func<String, String>>(Sm);
                var invoke = @delegate.Invoke("sd");
                Assert.AreEqual(invoke, "sd");
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [TestMethod]
        public void StaticSRet()
        {
            try
            {
                var @delegate = new WeakDelegate<Func<String, String>>(sSm);
                var invoke = @delegate.Invoke("sd");
                Assert.AreEqual(invoke, "sd");
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [TestMethod]
        public void StaticSRet2()
        {
            try
            {
                var @delegate = new WeakDelegate<Func<String, String>>(sSm);
                var del = new WeakDelegate<Func<String, String>>(@delegate);
                var invoke = del.Invoke("sd");
                Assert.AreEqual("sd", invoke);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [TestMethod]
        public void DynInvSret_2p()
        {
            try
            {
                var @delegate = new WeakDelegate<Func<String, int, String>>(Sm);
                var invoke = @delegate.DynamicInvoke("sd", 5);
                Assert.AreEqual(invoke, "sd");
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [TestMethod]
        public void VoidRet()
        {
            try
            {
                var @delegate = new WeakDelegate<Action<String, int>>(Vm);
                @delegate.Invoke("sd", 5);

            }
            catch (Exception e)
            {
                throw;
            }

        }

        [TestMethod]
        public void IntRet()
        {
            try
            {
                var @delegate = new WeakDelegate<Func<int, int>>(Im);
                var invoke = @delegate.Invoke(1);
                Assert.AreEqual(1, invoke);
            }
            catch (Exception e)
            {
                throw;
            }

        }

        [TestMethod]
        public void GuidRet()
        {
            try
            {
                var @delegate = new WeakDelegate<Func<Guid, Guid>>(Gm);
                var id = Guid.NewGuid();
                var invoke = @delegate.Invoke(id);
                Assert.AreEqual( id, invoke);
            }
            catch (Exception e)
            {
                throw ;
            }

        }

        public String Sm(String s)
        {
            return s;
        }
        public static String sSm(String s)
        {
            return s;
        }
        public String Sm(String s, int o)
        {
            return s;
        }
        public void Vm(String s, int o)
        {
        }
        public int Im(int o)
        {
            return o;
        }
        public Guid Gm(Guid o)
        {
            return o;
        }
    }
}
