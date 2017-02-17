using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using Buddy.Test.TestData;
using Buddy.Utility;
using Buddy.Web.TabularQuery;
using NUnit.Framework;

namespace Buddy45.Test.Web.TabularQuery
{
    public class FilterExpressionTest
    {
        [TestCase("customerName")]
        [TestCase("backlog")]
        [TestCase("pctPagingFileUsage")]
        [TestCase("lastReceivedOn")]
        public void SingleEqFilter(string fieldName, Type tp)
        {
            var data = PerformanceSnapshot.Load().AsQueryable();
            
            // Build a grouping to pull out a distinct value to use for the filter
            var expParam = Expression.Parameter(typeof(PerformanceSnapshot), "p");
            var accessor = Expression.Lambda<Func<PerformanceSnapshot, object>>(
                    Expression.MakeMemberAccess(expParam, typeof(PerformanceSnapshot).GetProperty(fieldName.ToUpperCamelCase())),
                    expParam)
                .Compile();
            
            var values = data.GroupBy(accessor, p => p).OrderBy(g => g.Key).ToList();
            
            Assert.That(values.Any());
                   
            var value = values[values.Count / 2];

            var filter = new FilterExpression
            {
                Logic = "and",
                Operator = "eq",
                Field = fieldName,
                Value = value.Key
            };

            var param = new List<object>();
            Console.WriteLine(filter.ToExpression(param));

            var result = data.Filter(filter);
           
            Assert.AreEqual(value.Count(), result.Count());
        }
    }
}