using NUnit.Framework;
using Overlord.Core.DataStructures;
using System;

namespace InfraDataStructureTest
{
    public class Tests
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
        }

        [Test]
        public void TestGetItem_WithoutOverflowIndex()
        {
            CircularList<int> intCircular = new CircularList<int>(10);

            for (int i = 0; i < 10; i++)
            {
                intCircular.AddItem(i);
            }
            
            Assert.AreEqual(5, intCircular.GetItem(5));
        }
        
        [Test]
        public void TestGetItem_OverflowList_WithoutOverflowIndex()
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

        private static string ConvertToString(CircularList<int> intCircular)
        {
            string result = String.Empty;
            
            foreach (int item in intCircular)
            {
                result += item.ToString();
                result += ", ";
            }
            result = result.TrimEnd(' ');
            result = result.TrimEnd(',');

            return result;
        }
    }
}