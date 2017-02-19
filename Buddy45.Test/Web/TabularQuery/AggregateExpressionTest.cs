using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Buddy.Web.TabularQuery;
using System.Linq.Dynamic;
using Buddy.Test.TestData;
using Buddy.Utility;
using NUnit.Framework;

namespace Buddy45.Test.Web.TabularQuery
{
    public class AggregateExpressionTest
    {
        private static List<TestType> TestData => new List<TestType>
        {
            new TestType { Id = 1, StringProperty = "String1", IntProperty = 100, DoubleProperty = 100.0 },
            new TestType { Id = 2, StringProperty = "String2", IntProperty = 200, DoubleProperty = 200.0 },
            new TestType { Id = 3, StringProperty = "String3", IntProperty = 300, DoubleProperty = 300.0 },
            new TestType { Id = 4, StringProperty = "String4", IntProperty = 400, DoubleProperty = 400.0 },
            new TestType { Id = 5, StringProperty = "String5", IntProperty = 500, DoubleProperty = 500.0 }
        };
        
        [Test]
        public void ComplexAggregate()
        {
            var data = PerformanceSnapshot.Load().AsQueryable();
            
            var aggregates = new List<AggregateExpression>
            {
                new AggregateExpression {Field = "customerName", Aggregate = AggregateExpression.Count},
                new AggregateExpression {Field = "backlog", Aggregate = AggregateExpression.Sum},
                new AggregateExpression {Field = "backlog", Aggregate = AggregateExpression.Average},
                new AggregateExpression {Field = "backlog", Aggregate = AggregateExpression.Max},
                new AggregateExpression {Field = "backlog", Aggregate = AggregateExpression.Min},
                new AggregateExpression {Field = "availableMBytes", Aggregate = AggregateExpression.Sum},
                new AggregateExpression {Field = "availableMBytes", Aggregate = AggregateExpression.Average},
                new AggregateExpression {Field = "availableMBytes", Aggregate = AggregateExpression.Max},
                new AggregateExpression {Field = "availableMBytes", Aggregate = AggregateExpression.Min},
                new AggregateExpression {Field = "pctPagingFileUsage", Aggregate = AggregateExpression.Sum},
                new AggregateExpression {Field = "pctPagingFileUsage", Aggregate = AggregateExpression.Max},
                new AggregateExpression {Field = "pctPagingFileUsage", Aggregate = AggregateExpression.Min},
                new AggregateExpression {Field = "totalReceived", Aggregate = AggregateExpression.Average},
                new AggregateExpression {Field = "totalReceived", Aggregate = AggregateExpression.Max},
                new AggregateExpression {Field = "totalReceived", Aggregate = AggregateExpression.Min}
            };
            
            var sw = new Stopwatch();
            sw.Start();
            var result = data.Aggregate(aggregates);
            sw.Stop();
            Console.WriteLine($"{nameof(ComplexAggregate)}: {sw.ElapsedMilliseconds}ms");
            /*
            var data = PerformanceSnapshot.Load().AsQueryable();
            
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