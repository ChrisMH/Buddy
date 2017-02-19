﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Buddy.Test.TestData;
using Buddy.Utility;
using Buddy.Web.TabularQuery;
using NUnit.Framework;

namespace Buddy45.Test.Web.TabularQuery
{
    public class QueryableMethodHelpers
    {
        private static MethodInfo GetMethod(string methodName, Func<Type[], Type[]> getParameterTypes, int genericArgumentCount)
        {
            var methods = from method in typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                let parameters = method.GetParameters()
                let genericArguments = method.GetGenericArguments()
                where method.Name == methodName &&
                      genericArguments.Length == genericArgumentCount &&
                      parameters.Select(p => p.ParameterType).SequenceEqual(getParameterTypes.Invoke(genericArguments))
                select method;
            return methods.FirstOrDefault();
        }
    }

    public class FilterExpressionTest
    {
        public static IEnumerable TestCases
        {
            get { yield return new TestCaseData("customerName"); }
        }

        /// <summary>
        ///     Returns an operator like p => p.fieldName oper value
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="oper"></param>
        /// <returns></returns>
        private Expression<Func<PerformanceSnapshot, bool>> GetComparison(string fieldName, string oper, object value)
        {
            var lmdParam = Expression.Parameter(typeof(PerformanceSnapshot), "p");

            switch (oper)
            {
                case FilterExpression.Eq:
                    // p => p.<fieldName> == <value>
                    return Expression.Lambda<Func<PerformanceSnapshot, bool>>(
                        Expression.Equal(Expression.Property(lmdParam, fieldName.ToUpperCamelCase()),
                            Expression.Constant(value)), lmdParam);
                case FilterExpression.Neq:
                    // p => p.<fieldName> != <value>
                    return Expression.Lambda<Func<PerformanceSnapshot, bool>>(
                        Expression.NotEqual(Expression.Property(lmdParam, fieldName.ToUpperCamelCase()),
                            Expression.Constant(value)), lmdParam);

                case FilterExpression.Lt:
                    // p => p.<fieldName> < <value>
                    return Expression.Lambda<Func<PerformanceSnapshot, bool>>(
                        Expression.LessThan(Expression.Property(lmdParam, fieldName.ToUpperCamelCase()),
                            Expression.Constant(value)), lmdParam);

                case FilterExpression.Lte:
                    // p => p.<fieldName> <= <value>
                    return Expression.Lambda<Func<PerformanceSnapshot, bool>>(
                        Expression.LessThanOrEqual(Expression.Property(lmdParam, fieldName.ToUpperCamelCase()),
                            Expression.Constant(value)), lmdParam);

                case FilterExpression.Gt:
                    // p => p.<fieldName> > <value>
                    return Expression.Lambda<Func<PerformanceSnapshot, bool>>(
                        Expression.GreaterThan(Expression.Property(lmdParam, fieldName.ToUpperCamelCase()),
                            Expression.Constant(value)), lmdParam);

                case FilterExpression.Gte:
                    // p => p.<fieldName> >= <value>
                    return Expression.Lambda<Func<PerformanceSnapshot, bool>>(
                        Expression.GreaterThanOrEqual(Expression.Property(lmdParam, fieldName.ToUpperCamelCase()),
                            Expression.Constant(value)), lmdParam);

                case FilterExpression.StartsWith:
                    return null;

                case FilterExpression.EndsWith:
                    return null;

                case FilterExpression.Contains:
                    return null;

                case FilterExpression.DoesNotContain:
                    return null;

                case FilterExpression.IsNull:
                    return null;

                case FilterExpression.IsNotNull:
                    return null;

                case FilterExpression.IsEmpty:
                    return null;

                case FilterExpression.IsNotEmpty:
                    return null;

                default:
                    throw new ArgumentException($"Unknown operator: {oper}", nameof(oper));
            }
        }

        [TestCase("customerName", FilterExpression.Eq)]
        [TestCase("backlog", FilterExpression.Eq)]
        [TestCase("pctPagingFileUsage", FilterExpression.Eq)]
        [TestCase("statTime", FilterExpression.Eq)]
        [TestCase("customerName", FilterExpression.Neq)]
        [TestCase("backlog", FilterExpression.Neq)]
        [TestCase("pctPagingFileUsage", FilterExpression.Neq)]
        [TestCase("statTime", FilterExpression.Neq)]
        [TestCase("backlog", FilterExpression.Lt)]
        [TestCase("pctPagingFileUsage", FilterExpression.Lt)]
        [TestCase("statTime", FilterExpression.Lt)]
        [TestCase("backlog", FilterExpression.Lte)]
        [TestCase("pctPagingFileUsage", FilterExpression.Lte)]
        [TestCase("statTime", FilterExpression.Lte)]
        [TestCase("backlog", FilterExpression.Gt)]
        [TestCase("pctPagingFileUsage", FilterExpression.Gt)]
        [TestCase("statTime", FilterExpression.Gt)]
        [TestCase("backlog", FilterExpression.Gte)]
        [TestCase("pctPagingFileUsage", FilterExpression.Gte)]
        [TestCase("statTime", FilterExpression.Gte)]
        public void SingleComparisonFilter(string fieldName, string oper)
        {
            var data = PerformanceSnapshot.Load().AsQueryable();

            // Build a grouping to pull out a distinct value to use for the filter
            var lmdParam = Expression.Parameter(typeof(PerformanceSnapshot), "p");

            // p => p.<fieldName>
            var lmdAccessor = Expression.Lambda<Func<PerformanceSnapshot, object>>(
                Expression.Convert(
                    Expression.MakeMemberAccess(lmdParam, typeof(PerformanceSnapshot).GetProperty(fieldName.ToUpperCamelCase())),
                    typeof(object)),
                lmdParam);

            var values = data.GroupBy(lmdAccessor, p => p).Select(g => g.Key).OrderBy(k => k).ToList();
            Assert.That(values.Any());
            var value = values[values.Count / 2];

            var filter = new FilterExpression
            {
                Logic = "and",
                Filters = new List<FilterExpression>
                {
                    new FilterExpression
                    {
                        Operator = oper,
                        Field = fieldName,
                        Value = value.ToString()
                    }
                }
            };

            var param = new List<object>();
            Console.WriteLine(filter.ToExpression<PerformanceSnapshot>(param));

            var result = data.Filter(filter);

            var lmdFilter = GetComparison(fieldName, oper, value);
            var expectedCount = data.Count(lmdFilter);

            Assert.AreEqual(expectedCount, result.Count());
        }


        [TestCase("customerName")]
        public void SingleStartsWithFilter(string fieldName)
        {
        }

        [TestCase("customerName")]
        public void SingleEndsWithFilter(string fieldName)
        {
        }

        [TestCase("customerName")]
        public void SingleContainsFilter(string fieldName)
        {
        }

        [TestCase("customerName")]
        public void SingleDoesNotContainFilter(string fieldName)
        {
        }

        [TestCase("stringProperty")]
        [TestCase("nullableIntProperty")]
        public void SingleIsNullFilter(string fieldName)
        {
            var data = new List<TestType>
            {
                new TestType {Id = 0, StringProperty = "String1", NullableIntProperty = 1},
                new TestType {Id = 1, StringProperty = null, NullableIntProperty = null}
            }.AsQueryable();

            var filter = new FilterExpression
            {
                Logic = "and",
                Filters = new List<FilterExpression>
                {
                    new FilterExpression
                    {
                        Operator = FilterExpression.IsNull,
                        Field = fieldName
                    }
                }
            };

            var param = new List<object>();
            Console.WriteLine(filter.ToExpression<TestType>(param));

            var filtered = data.Filter(filter);

            Assert.AreEqual(1, filtered.Count());
            Assert.AreEqual(1, filtered.First().Id);
        }

        [TestCase("stringProperty")]
        public void SingleIsNotNullFilter(string fieldName)
        {
            var data = new List<TestType>
            {
                new TestType {Id = 0, StringProperty = "String1", NullableIntProperty = 1},
                new TestType {Id = 1, StringProperty = null, NullableIntProperty = null}
            }.AsQueryable();

            var filter = new FilterExpression
            {
                Logic = "and",
                Filters = new List<FilterExpression>
                {
                    new FilterExpression
                    {
                        Operator = FilterExpression.IsNotNull,
                        Field = fieldName
                    }
                }
            };

            var param = new List<object>();
            Console.WriteLine(filter.ToExpression<TestType>(param));

            var filtered = data.Filter(filter);

            Assert.AreEqual(1, filtered.Count());
            Assert.AreEqual(0, filtered.First().Id);
        }

        [TestCase("stringProperty")]
        public void SingleIsEmptyFilter(string fieldName)
        {
            var data = new List<TestType>
            {
                new TestType {Id = 0, StringProperty = "String1"},
                new TestType {Id = 1, StringProperty = ""}
            }.AsQueryable();

            var filter = new FilterExpression
            {
                Logic = "and",
                Filters = new List<FilterExpression>
                {
                    new FilterExpression
                    {
                        Operator = FilterExpression.IsEmpty,
                        Field = fieldName
                    }
                }
            };

            var param = new List<object>();
            Console.WriteLine(filter.ToExpression<TestType>(param));

            var filtered = data.Filter(filter);

            Assert.AreEqual(1, filtered.Count());
            Assert.AreEqual(1, filtered.First().Id);
        }

        [TestCase("stringProperty")]
        public void SingleIsNotEmptyFilter(string fieldName)
        {
            var data = new List<TestType>
            {
                new TestType {Id = 0, StringProperty = "String1"},
                new TestType {Id = 1, StringProperty = ""}
            }.AsQueryable();

            var filter = new FilterExpression
            {
                Logic = "and",
                Filters = new List<FilterExpression>
                {
                    new FilterExpression
                    {
                        Operator = FilterExpression.IsNotEmpty,
                        Field = fieldName
                    }
                }
            };

            var param = new List<object>();
            Console.WriteLine(filter.ToExpression<TestType>(param));

            var filtered = data.Filter(filter);

            Assert.AreEqual(1, filtered.Count());
            Assert.AreEqual(0, filtered.First().Id);
        }


        public class TestType
        {
            public int Id { get; set; }
            public string StringProperty { get; set; }
            public int? NullableIntProperty { get; set; }
        }
    }
}