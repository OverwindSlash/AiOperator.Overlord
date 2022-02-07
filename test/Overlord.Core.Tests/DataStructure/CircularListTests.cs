using System;
using System.Text;
using NSubstitute;
using NUnit.Framework;
using Overlord.Core.DataStructures;

namespace Overlord.Core.Tests.DataStructure
{
    public class CircularListTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestAddItem_WithoutOverflow()
        {
            CircularList<int> intCircular = new CircularList<int>(10);

            for (int i = 0; i < 10; i++)
            {
                intCircular.AddItem(i);
            }

            var result = ConvertToString(intCircular);

            Assert.AreEqual("0, 1, 2, 3, 4, 5, 6, 7, 8, 9", result);
            Assert.AreEqual(0, intCircular.CurrentIndex);
        }
        
        [Test]
        public void TestAddItem_WithOverflow1()
        {
            CircularList<int> intCircular = new CircularList<int>(10);
            
            for (int i = 0; i < 11; i++)
            {
                intCircular.AddItem(i);
            }
            
            var result = ConvertToString(intCircular);
            
            Assert.AreEqual("10, 1, 2, 3, 4, 5, 6, 7, 8, 9", result);
            Assert.AreEqual(1, intCircular.CurrentIndex);
        }
        
        [Test]
        public void TestAddItem_WithOverflow10()
        {
            CircularList<int> intCircular = new CircularList<int>(10);
            
            for (int i = 0; i < 20; i++)
            {
                intCircular.AddItem(i);
            }
            
            var result = ConvertToString(intCircular);
            
            Assert.AreEqual("10, 11, 12, 13, 14, 15, 16, 17, 18, 19", result);
            Assert.AreEqual(0, intCircular.CurrentIndex);
        }

        [Test]
        public void TestGetItem_WithNoOverflowIndex()
        {
            CircularList<int> intCircular = new CircularList<int>(10);

            for (int i = 0; i < 10; i++)
            {
                intCircular.AddItem(i);
            }
            
            Assert.AreEqual(5, intCircular.GetItem(5));
        }
        
        [Test]
        public void TestGetItem_OverflowList_WithNoOverflowIndex()
        {
            CircularList<int> intCircular = new CircularList<int>(10);

            for (int i = 0; i < 20; i++)
            {
                intCircular.AddItem(i);
            }
            
            Assert.AreEqual(15, intCircular.GetItem(5));
        }
        
        [Test]
        public void TestGetItem_WithOverflowIndex()
        {
            CircularList<int> intCircular = new CircularList<int>(10);

            for (int i = 0; i < 10; i++)
            {
                intCircular.AddItem(i);
            }
            
            Assert.AreEqual(5, intCircular.GetItem(15));
        }
        
        [Test]
        public void TestGetItem_WithWrongIndex()
        {
            CircularList<int> intCircular = new CircularList<int>(10);

            for (int i = 0; i < 10; i++)
            {
                intCircular.AddItem(i);
            }
            
            Assert.AreEqual(0, intCircular.GetItem(-1));
        }
        
        [Test]
        public void TestGetItem_OverflowList_WithOverflowIndex()
        {
            CircularList<int> intCircular = new CircularList<int>(10);

            for (int i = 0; i < 20; i++)
            {
                intCircular.AddItem(i);
            }
            
            Assert.AreEqual(14, intCircular.GetItem(14));
        }
        
        [Test]
        public void TestAddItem_Disposable_WithExceedSizeLimit1()
        {
            CircularList<IDisposable> intCircular = new CircularList<IDisposable>(1);

            IDisposable obj1 = Substitute.For<IDisposable>();
            IDisposable obj2 = Substitute.For<IDisposable>();
            
            intCircular.AddItem(obj1);
            intCircular.AddItem(obj2);

            obj1.Received().Dispose();
            obj2.DidNotReceive().Dispose();
        }
        
        [Test]
        public void TestAddItem_AddNull()
        {
            CircularList<IDisposable> intCircular = new CircularList<IDisposable>(1);
            
            IDisposable obj1 = Substitute.For<IDisposable>();
            IDisposable obj2 = null;
            
            intCircular.AddItem(obj1);
            intCircular.AddItem(obj2);
            
            obj1.DidNotReceive().Dispose();
        }

        private static string ConvertToString(CircularList<int> intCircular)
        {
            StringBuilder builder = new StringBuilder();
            foreach (int item in intCircular)
            {
                builder.Append(item.ToString());
                builder.Append(", ");
            }

            string result = builder.ToString();
            result = result.TrimEnd(' ');
            result = result.TrimEnd(',');

            return result;
        }
    }
}