using System;
using System.Collections.Generic;
using System.Linq;
using Buddy.Test.TestData;
using Buddy.Web.TabularQuery;
using NUnit.Framework;

namespace Buddy45.Test.Web.TabularQuery
{
    public class AggregateExpressionTest
    {
        [Test]
        public void CountAggregateReturnsExpectedResult()
        {
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
        }
    }
}