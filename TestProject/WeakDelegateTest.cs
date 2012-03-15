using System;
using NUnit.Framework;
using SUF.Common.GeneralPurpose;

namespace TestProject
{
    [TestFixture]
    public class WeakDelegateTest
    {

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

        [Test]
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

        [Test]
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

        [Test]
        public void ManySubscribers()
        {
            try
            {
                int count = 5;
                int I = 0;
                var @delegate = new WeakDelegate<Action>();
                for(int i=0; i< count;i++)
                    @delegate +=()=>I++ ;
                @delegate.Invoke();
                Assert.AreEqual(count, I);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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
