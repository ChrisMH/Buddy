using System;
using System.Linq;
using System.Linq.Dynamic;
using System.Collections.Generic;
using NUnit.Framework;

namespace Buddy45.Test.Web.TabularQuery
{
    public class PlaygroundTest
    {
        private static List<TestType> TestData => new List<TestType>
        {
            new TestType {Id = 1, StringProperty = "String1", ShortProperty = 100, IntProperty = Int32.MaxValue, FloatProperty = float.MaxValue, DoubleProperty = 100.0},
            new TestType {Id = 2, StringProperty = "String2", ShortProperty = 200, IntProperty = 2, FloatProperty = 200.0f, DoubleProperty = 200.0},
            new TestType {Id = 3, StringProperty = "String3", ShortProperty = 300, IntProperty = 3, FloatProperty = 300.0f, DoubleProperty = 300.0},
            new TestType {Id = 4, StringProperty = "String4", ShortProperty = 400, IntProperty = 4, FloatProperty = 400.0f, DoubleProperty = 400.0},
            new TestType {Id = 5, StringProperty = "String5", ShortProperty = 500, IntProperty = 5, FloatProperty = 500.0f, DoubleProperty = 500.0}
        };

        [Test]
        public void TestSum() {

            var linqSum = TestData.Sum(x => Convert.ToInt64(x.IntProperty));


            Assert.AreEqual(Convert.ToInt64(Int32.MaxValue) + 14, linqSum);

            var result = TestData.AsQueryable<TestType>().GroupBy(g => 1).Select("Sum(IntProperty)");
        }

        [Test]
        public void TestCount()
        {

            var linqSum = TestData.Count();

            Assert.AreEqual(5, linqSum);
        }


        public long BuildSum<T>(string propertyName) {
            

            return 0;
        }
    }
}