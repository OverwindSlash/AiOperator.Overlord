using System;
using System.Text;
using NUnit.Framework;
using Overlord.Core.DataStructures;

namespace Overlord.Core.Tests.DataStructure
{
    public class FixedSizeQueueTests
    {
        [Test]
        public void TestEnqueue_WithoutExceedSizeLimit()
        {
            int sizeLimit = 10;
            FixedSizeQueue<int> queue = new FixedSizeQueue<int>(sizeLimit);

            for (int i = 0; i < sizeLimit; i++)
            {
                queue.Enqueue(i);
            }
            
            var result = ConvertToString(queue);
            
            Assert.AreEqual("0, 1, 2, 3, 4, 5, 6, 7, 8, 9", result);
        }
        
        [Test]
        public void TestEnqueue_DefaultPredicate_WithoutExceedSizeLimit()
        {
            int sizeLimit = 10;
            FixedSizeQueue<int> queue = new FixedSizeQueue<int>(sizeLimit);

            for (int i = 0; i < sizeLimit; i++)
            {
                queue.Enqueue(i);
            }
            
            Assert.IsTrue(queue.IsPositive());
        }
        
        [Test]
        public void TestEnqueue_WrongSize_WithoutExceedSizeLimit()
        {
            int sizeLimit = -1;
            Assert.Catch<ArgumentException>(() => new FixedSizeQueue<int>(sizeLimit));
        }

        [Test]
        public void TestEnqueue_WithExceedSizeLimit1()
        {
            int sizeLimit = 10;
            FixedSizeQueue<int> queue = new FixedSizeQueue<int>(sizeLimit);

            for (int i = 0; i < sizeLimit + 1; i++)
            {
                queue.Enqueue(i);
            }
            
            var result = ConvertToString(queue);
            
            Assert.AreEqual("1, 2, 3, 4, 5, 6, 7, 8, 9, 10", result);
        }
        
        [Test]
        public void TestDequeue_WithExceedSizeLimit3()
        {
            int sizeLimit = 10;
            FixedSizeQueue<int> queue = new FixedSizeQueue<int>(sizeLimit);

            for (int i = 0; i < sizeLimit + 3; i++)
            {
                queue.Enqueue(i);
            }
            
            var result = ConvertToString(queue);
            
            Assert.AreEqual("3, 4, 5, 6, 7, 8, 9, 10, 11, 12", result);
            
            Assert.AreEqual(3, queue.Dequeue());
        }
        
        [Test]
        public void TestEnqueue_WithExceedSizeLimit10()
        {
            int sizeLimit = 10;
            FixedSizeQueue<int> queue = new FixedSizeQueue<int>(sizeLimit);

            for (int i = 0; i < sizeLimit + 10; i++)
            {
                queue.Enqueue(i);
            }
            
            var result = ConvertToString(queue);
            
            Assert.AreEqual("10, 11, 12, 13, 14, 15, 16, 17, 18, 19", result);
        }
        
        [Test]
        public void TestPredicate_WithoutExceedSizeLimit_OverThresh()
        {
            int sizeLimit = 10;
            FixedSizeQueue<int> queue = new FixedSizeQueue<int>(sizeLimit, 
                i => i % 2 == 0, 0.4);

            for (int i = 0; i < sizeLimit + 10; i++)
            {
                queue.Enqueue(i);
            }

            Assert.True(queue.IsPositive());
        }
        
        [Test]
        public void TestPredicate_WithoutExceedSizeLimit_EqualThresh()
        {
            int sizeLimit = 10;
            FixedSizeQueue<int> queue = new FixedSizeQueue<int>(sizeLimit, 
                i => i % 2 == 0, 0.5);

            for (int i = 0; i < sizeLimit + 10; i++)
            {
                queue.Enqueue(i);
            }

            Assert.True(queue.IsPositive());
        }
        
        [Test]
        public void TestPredicate_WithoutExceedSizeLimit_BelowThresh()
        {
            int sizeLimit = 10;
            FixedSizeQueue<int> queue = new FixedSizeQueue<int>(sizeLimit, 
                i => i % 2 == 0, 0.6);

            for (int i = 0; i < sizeLimit + 10; i++)
            {
                queue.Enqueue(i);
            }

            Assert.False(queue.IsPositive());
        }
        
        [Test]
        public void TestPredicate_WithExceedSizeLimit_DecreacePositiveItemCount()
        {
            int sizeLimit = 10;
            FixedSizeQueue<int> queue = new FixedSizeQueue<int>(sizeLimit, 
                i => i % 2 == 0, 0.5);

            for (int i = 0; i < sizeLimit; i++)
            {
                queue.Enqueue(i);
            }
            
            Assert.AreEqual(5, queue.PositiveItemCount);
            
            queue.Enqueue(11);
            
            Assert.AreEqual(4, queue.PositiveItemCount);

            Assert.False(queue.IsPositive());
        }
        
        [Test]
        public void TestPredicate_WithOutExceedSizeLimit_DequeueItem()
        {
            int sizeLimit = 10;
            FixedSizeQueue<int> queue = new FixedSizeQueue<int>(sizeLimit, 
                i => i % 2 == 0, 0.4);

            for (int i = 0; i < sizeLimit; i++)
            {
                queue.Enqueue(i);
            }
            
            Assert.AreEqual(5, queue.PositiveItemCount);

            var item = queue.Dequeue();

            Assert.AreEqual(0, item);
            Assert.AreEqual(4, queue.PositiveItemCount);
            Assert.False(queue.IsPositive());
            
            queue.Enqueue(11);
            
            Assert.AreEqual(4, queue.PositiveItemCount);
            Assert.IsTrue(queue.IsPositive());
        }

        [Test]
        public void TestPeek()
        {
            int sizeLimit = 10;
            FixedSizeQueue<int> queue = new FixedSizeQueue<int>(sizeLimit);

            for (int i = 0; i < sizeLimit; i++)
            {
                queue.Enqueue(i);
            }

            bool result = queue.Peek(out int value);
            
            Assert.IsTrue(result);
            Assert.AreEqual(0, value);
            Assert.AreEqual(10, queue.Count());
        }

        private static string ConvertToString(FixedSizeQueue<int> fixedSizeQueue)
        {
            StringBuilder builder = new StringBuilder();
            foreach (int item in fixedSizeQueue)
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