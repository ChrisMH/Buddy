using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Buddy.Test.PerformanceTestData;
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
        private Expression<Func<PerformanceEntry, bool>> GetComparison(string fieldName, string oper, object value)
        {
            var lmdParam = Expression.Parameter(typeof(PerformanceEntry), "p");

            switch (oper)
            {
                case FilterExpression.Eq:
                    // p => p.<fieldName> == <value>
                    return Expression.Lambda<Func<PerformanceEntry, bool>>(
                        Expression.Equal(Expression.Property(lmdParam, fieldName.ToUpperCamelCase()),
                            Expression.Constant(value)), lmdParam);
                case FilterExpression.Neq:
                    // p => p.<fieldName> != <value>
                    return Expression.Lambda<Func<PerformanceEntry, bool>>(
                        Expression.NotEqual(Expression.Property(lmdParam, fieldName.ToUpperCamelCase()),
                            Expression.Constant(value)), lmdParam);

                case FilterExpression.Lt:
                    // p => p.<fieldName> < <value>
                    return Expression.Lambda<Func<PerformanceEntry, bool>>(
                        Expression.LessThan(Expression.Property(lmdParam, fieldName.ToUpperCamelCase()),
                            Expression.Constant(value)), lmdParam);

                case FilterExpression.Lte:
                    // p => p.<fieldName> <= <value>
                    return Expression.Lambda<Func<PerformanceEntry, bool>>(
                        Expression.LessThanOrEqual(Expression.Property(lmdParam, fieldName.ToUpperCamelCase()),
                            Expression.Constant(value)), lmdParam);

                case FilterExpression.Gt:
                    // p => p.<fieldName> > <value>
                    return Expression.Lambda<Func<PerformanceEntry, bool>>(
                        Expression.GreaterThan(Expression.Property(lmdParam, fieldName.ToUpperCamelCase()),
                            Expression.Constant(value)), lmdParam);

                case FilterExpression.Gte:
                    // p => p.<fieldName> >= <value>
                    return Expression.Lambda<Func<PerformanceEntry, bool>>(
                        Expression.GreaterThanOrEqual(Expression.Property(lmdParam, fieldName.ToUpperCamelCase()),
                            Expression.Constant(value)), lmdParam);

                case FilterExpression.StartsWith:
                    {
                        // p => p.<fieldName>.StartsWith(<value>)
                        var p = Expression.Property(lmdParam, fieldName.ToUpperCamelCase());
                        var c = Expression.Constant(value);
                        var method = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });
                        var methodCall = Expression.Call(p, method, c);

                        return Expression.Lambda<Func<PerformanceEntry, bool>>(methodCall, lmdParam);
                    }
                
                case FilterExpression.EndsWith:
                    return null;

                case FilterExpression.Contains:
                    {
                        // p => p.<fieldName>.StartsWith(<value>)
                        var p = Expression.Property(lmdParam, fieldName.ToUpperCamelCase());
                        var c = Expression.Constant(value);
                        var method = typeof(string).GetMethod("IndexOf", new Type[] { typeof(string) });
                        var methodCall = Expression.Call(p, method, c);

                        return Expression.Lambda<Func<PerformanceEntry, bool>>(Expression.GreaterThanOrEqual(methodCall, Expression.Constant(0)), lmdParam);
                    }

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

        public object GetValueForComparison(IQueryable<PerformanceEntry> data, string fieldName, string oper)
        {
            // Build a grouping to pull out a distinct value to use for the filter
            var lmdParam = Expression.Parameter(typeof(PerformanceEntry), "p");

            // p => p.<fieldName>
            var lmdAccessor = Expression.Lambda<Func<PerformanceEntry, object>>(
                Expression.Convert(
                    Expression.MakeMemberAccess(lmdParam, typeof(PerformanceEntry).GetProperty(fieldName.ToUpperCamelCase())),
                    typeof(object)),
                lmdParam);

            var values = data.GroupBy(lmdAccessor, p => p).Select(g => g.Key).OrderBy(k => k).ToList();

            Assert.That(values.Any());
            var value = values[values.Count / 2];

            if(value.GetType() != typeof(string))
            {
                return value;
            }

            if(oper == FilterExpression.StartsWith)
            {
                return ((string)value).Substring(0, ((string)value).Length / 2);
            }
            else if(oper == FilterExpression.EndsWith)
            {
                return ((string)value).Substring(((string)value).Length / 2);
            }
            else if(oper == FilterExpression.Contains)
            {
                return ((string)value).Substring(1, ((string)value).Length - 2);
            }
            else if(oper == FilterExpression.DoesNotContain)
            {
                return ((string)value).Substring(1, ((string)value).Length - 2);
            }

            return value;
        }


        [TestCase("customerName", FilterExpression.Eq)]
        [TestCase("customerName", FilterExpression.Eq, true)]
        [TestCase("customerName", FilterExpression.Neq)]
        [TestCase("customerName", FilterExpression.Neq, true)]
        [TestCase("customerName", FilterExpression.StartsWith)]
        [TestCase("customerName", FilterExpression.StartsWith, true)]
        [TestCase("customerName", FilterExpression.Contains)]
        [TestCase("customerName", FilterExpression.Contains, true)]

        [TestCase("availableMBytes", FilterExpression.Eq)]
        [TestCase("pctPagingFileUsage", FilterExpression.Eq)]
        [TestCase("statTime", FilterExpression.Eq)]
        [TestCase("availableMBytes", FilterExpression.Neq)]
        [TestCase("pctPagingFileUsage", FilterExpression.Neq)]
        [TestCase("statTime", FilterExpression.Neq)]
        [TestCase("availableMBytes", FilterExpression.Lt)]
        [TestCase("pctPagingFileUsage", FilterExpression.Lt)]
        [TestCase("statTime", FilterExpression.Lt)]
        [TestCase("availableMBytes", FilterExpression.Lte)]
        [TestCase("pctPagingFileUsage", FilterExpression.Lte)]
        [TestCase("statTime", FilterExpression.Lte)]
        [TestCase("availableMBytes", FilterExpression.Gt)]
        [TestCase("pctPagingFileUsage", FilterExpression.Gt)]
        [TestCase("statTime", FilterExpression.Gt)]
        [TestCase("availableMBytes", FilterExpression.Gte)]
        [TestCase("pctPagingFileUsage", FilterExpression.Gte)]
        [TestCase("statTime", FilterExpression.Gte)]
        public void SingleComparisonFilter(string fieldName, string oper, bool modifyCase = false)
        {
            var data = PerformanceEntry.Load().AsQueryable();

            var value = GetValueForComparison(data, fieldName, oper);

            var filter = new FilterExpression
            {
                Logic = "and",
                Filters = new List<FilterExpression>
                {
                    new FilterExpression
                    {
                        Operator = oper,
                        Field = fieldName,
                        Value = modifyCase ? value.ToString().ToLower() : value.ToString()
                    }
                }
            };

            var param = new List<object>();
            Console.WriteLine(filter.ToExpression<PerformanceEntry>(param));

            var result = data.Filter(filter);

            var lmdFilter = GetComparison(fieldName, oper, value);
            var expectedCount = data.Count(lmdFilter);

            Assert.AreEqual(expectedCount, result.Count());
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
        
    }
}