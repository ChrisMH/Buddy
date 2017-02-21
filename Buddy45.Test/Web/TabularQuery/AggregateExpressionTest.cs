using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Buddy.Web.TabularQuery;
using System.Linq.Dynamic;
using Buddy.Test.PerformanceTestData;
using NUnit.Framework;

namespace Buddy45.Test.Web.TabularQuery
{
    public class AggregateExpressionTest
    {
        private static List<TestType> TestData => new List<TestType>
        {
            new TestType { Id = 1, StringProperty = "String1", ShortProperty = 100, IntProperty = 100, FloatProperty = 100.0f, DoubleProperty = 100.0 },
            new TestType { Id = 2, StringProperty = "String2", ShortProperty = 200, IntProperty = 200, FloatProperty = 200.0f, DoubleProperty = 200.0 },
            new TestType { Id = 3, StringProperty = "String3", ShortProperty = 300, IntProperty = 300, FloatProperty = 300.0f, DoubleProperty = 300.0 },
            new TestType { Id = 4, StringProperty = "String4", ShortProperty = 400, IntProperty = 400, FloatProperty = 400.0f, DoubleProperty = 400.0 },
            new TestType { Id = 5, StringProperty = "String5", ShortProperty = 500, IntProperty = 500, FloatProperty = 500.0f, DoubleProperty = 500.0 }
        };

        [TestCase("stringProperty", AggregateExpression.Count, typeof(int), 5)]
        
        [TestCase("shortProperty", AggregateExpression.Count, typeof(int), 5)]
        [TestCase("shortProperty", AggregateExpression.Sum, typeof(int), 1500)]
        [TestCase("shortProperty", AggregateExpression.Min, typeof(short), 100)]
        [TestCase("shortProperty", AggregateExpression.Max, typeof(short), 500)]
        [TestCase("shortProperty", AggregateExpression.Average, typeof(double), 300)]

        [TestCase("intProperty", AggregateExpression.Count, typeof(int), 5)]
        [TestCase("intProperty", AggregateExpression.Sum, typeof(int), 1500)]
        [TestCase("intProperty", AggregateExpression.Min, typeof(int), 100)]
        [TestCase("intProperty", AggregateExpression.Max, typeof(int), 500)]
        [TestCase("intProperty", AggregateExpression.Average, typeof(double), 300)]
        
        [TestCase("floatProperty", AggregateExpression.Count, typeof(int), 5)]
        [TestCase("floatProperty", AggregateExpression.Sum, typeof(float), 1500.0f)]
        [TestCase("floatProperty", AggregateExpression.Min, typeof(float), 100.0f)]
        [TestCase("floatProperty", AggregateExpression.Max, typeof(float), 500.0f)]
        [TestCase("floatProperty", AggregateExpression.Average, typeof(float), 300.0f)]

        [TestCase("doubleProperty", AggregateExpression.Count, typeof(int), 5)]
        [TestCase("doubleProperty", AggregateExpression.Sum, typeof(double), 1500.0)]
        [TestCase("doubleProperty", AggregateExpression.Min, typeof(double), 100.0)]
        [TestCase("doubleProperty", AggregateExpression.Max, typeof(double), 500.0)]
        [TestCase("doubleProperty", AggregateExpression.Average, typeof(double), 300.0)]
        public void SingleAggregate(string field, string aggregate, Type expectedType, object expectedValue)
        {
            var data = TestData.AsQueryable();

            var aggregateExpression = new List<AggregateExpression>
            {
                new AggregateExpression {Field = field, Aggregate = aggregate}
            };

            var result = data.Aggregate(aggregateExpression);

            Assert.IsInstanceOf(typeof(object), result);

            var fieldProperty = result.GetType().GetProperty(field);
            Assert.NotNull(fieldProperty);

            var fieldInstance = fieldProperty.GetValue(result);
            Assert.NotNull(fieldInstance);

            var aggregateProperty = fieldProperty.PropertyType.GetProperty(aggregate);
            Assert.NotNull(aggregateProperty);

            var aggregateInstance = aggregateProperty.GetValue(fieldInstance);
            Assert.NotNull(aggregateInstance);
            Assert.IsAssignableFrom(expectedType, aggregateInstance);
            Assert.AreEqual(expectedValue, aggregateInstance);
        }
        


        [Test]
        public void ComplexAggregate()
        {
            var data = PerformanceEntry.Load().AsQueryable();
            
            var aggregates = new List<AggregateExpression>
            {
                new AggregateExpression {Field = "customerName", Aggregate = AggregateExpression.Count},
                new AggregateExpression {Field = "databaseConnections", Aggregate = AggregateExpression.Sum},
                new AggregateExpression {Field = "databaseConnections", Aggregate = AggregateExpression.Average},
                new AggregateExpression {Field = "databaseConnections", Aggregate = AggregateExpression.Max},
                new AggregateExpression {Field = "databaseConnections", Aggregate = AggregateExpression.Min},
                new AggregateExpression {Field = "totalDatabaseConnections", Aggregate = AggregateExpression.Sum},
                new AggregateExpression {Field = "totalDatabaseConnections", Aggregate = AggregateExpression.Average},
                new AggregateExpression {Field = "totalDatabaseConnections", Aggregate = AggregateExpression.Max},
                new AggregateExpression {Field = "totalDatabaseConnections", Aggregate = AggregateExpression.Min},
                new AggregateExpression {Field = "availableMBytes", Aggregate = AggregateExpression.Sum},
                new AggregateExpression {Field = "availableMBytes", Aggregate = AggregateExpression.Average},
                new AggregateExpression {Field = "availableMBytes", Aggregate = AggregateExpression.Max},
                new AggregateExpression {Field = "availableMBytes", Aggregate = AggregateExpression.Min},
                new AggregateExpression {Field = "pctPagingFileUsage", Aggregate = AggregateExpression.Sum},
                new AggregateExpression {Field = "pctPagingFileUsage", Aggregate = AggregateExpression.Max},
                new AggregateExpression {Field = "pctPagingFileUsage", Aggregate = AggregateExpression.Min}
            };
            
            var sw = new Stopwatch();
            sw.Start();
            var result = data.Aggregate(aggregates);
            sw.Stop();
            Console.WriteLine($"{nameof(ComplexAggregate)}: {sw.ElapsedMilliseconds}ms");
            /*
            var data = PerformanceEntry.Load().AsQueryable();
            
            var aggregate = new List<AggregateExpression>
            {
                new AggregateExpression { Field = "customerName", Aggregate = AggregateExpression.Count }
            };

            var result = data.Aggregate(aggregate);

            
            var cnProp = result.GetType().GetProperty("customerName");
            Assert.NotNull(cnProp);

            var cn = cnProp.GetValue(result);
            Assert.NotNull(cn);

            var countProp = cn.GetType().GetProperty(AggregateExpression.Count);
            Assert.NotNull(countProp);

            var count = countProp.GetValue(cn);

            Assert.IsAssignableFrom(typeof(int), count);
            

            Assert.AreEqual(data.Count(), Convert.ToInt32(count));
            */
        }
    }
}