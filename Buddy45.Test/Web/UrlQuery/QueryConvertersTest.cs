using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Buddy.Web.UrlQuery;

namespace Buddy.Test.Web.UrlQuery
{
    public class QueryConvertersTest
    {
        [TestCase("I am a string", "sm=I am a string")]
        public void CanConvert_String_ToUrl(string value, string expected)
        {
            var testClass = new TestClass {StringMember = value};

            var pi = testClass.GetType().GetProperties().First(p => p.Name == "StringMember");
            Assert.NotNull(pi);

            var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
            Assert.NotNull(queryAttr);
            Assert.True(queryAttr.Converter is StringConverter);

            var result = queryAttr.Converter.ToUrl(testClass, pi, queryAttr);

            Assert.AreEqual(expected, result);
        }

        [TestCase("sm=I am a string", "I am a string")]
        public void CanConvert_String_FromUrl(string value, string expected)
        {
            var testClass = new TestClass();
            var testSource = new Dictionary<string, string> {{value.Split('=')[0], value.Split('=')[1]}};

            var testData = new Dictionary<string, string> {{"sm", value}};
            
            var pi = testClass.GetType().GetProperties().First(p => p.Name == "StringMember");
            Assert.NotNull(pi);

            var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
            Assert.NotNull(queryAttr);
            Assert.True(queryAttr.Converter is StringConverter);

            queryAttr.Converter.FromUrl(testSource, testClass, pi, queryAttr);

            Assert.AreEqual(expected, testClass.StringMember);
        }

        [TestCase(0, "im=0")]
        [TestCase(192, "im=192")]
        [TestCase(-23, "im=-23")]
        public void CanConvert_Int_ToUrl(int value, string expected)
        {
            var testClass = new TestClass {Int32Member = value};
            
            var pi = testClass.GetType().GetProperties().First(p => p.Name == "Int32Member");
            Assert.NotNull(pi);

            var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
            Assert.NotNull(queryAttr);
            Assert.True(queryAttr.Converter is Int32Converter);

            var result = queryAttr.Converter.ToUrl(testClass, pi, queryAttr);

            Assert.AreEqual(expected, result);
        }

        [TestCase("im=0", 0)]
        [TestCase("im=192", 192)]
        [TestCase("im=-23", -23)]
        public void CanConvert_Int_FromUrl(string value, int expected)
        {
            var testClass = new TestClass();
            var testSource = new Dictionary<string, string> {{value.Split('=')[0], value.Split('=')[1]}};
            
            var pi = testClass.GetType().GetProperties().First(p => p.Name == "Int32Member");
            Assert.NotNull(pi);

            var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
            Assert.NotNull(queryAttr);
            Assert.True(queryAttr.Converter is Int32Converter);

            queryAttr.Converter.FromUrl(testSource, testClass, pi, queryAttr);

            Assert.AreEqual(expected, testClass.Int32Member);
        }

        [TestCase(true, "bm=t")]
        [TestCase(false, "bm=f")]
        public void CanConvert_Bool_ToUrl(bool value, string expected)
        {
            var testClass = new TestClass {BoolMember = value};
            
            var pi = testClass.GetType().GetProperties().First(p => p.Name == "BoolMember");
            Assert.NotNull(pi);

            var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
            Assert.NotNull(queryAttr);
            Assert.True(queryAttr.Converter is BoolConverter);

            var result = queryAttr.Converter.ToUrl(testClass, pi, queryAttr);

            Assert.AreEqual(expected, result);
        }
        
        [TestCase("bm=t", true)]
        [TestCase("bm=true", true)]
        [TestCase("bm=True", true)]
        [TestCase("bm=booger", false)]
        [TestCase("bm=f", false)]
        [TestCase("bm=false", false)]
        [TestCase("bm=False", false)]
        public void CanConvert_Bool_FromUrl(string value, bool expected)
        {
            var testClass = new TestClass();
            var testSource = new Dictionary<string, string> {{value.Split('=')[0], value.Split('=')[1]}};
            
            var pi = testClass.GetType().GetProperties().First(p => p.Name == "BoolMember");
            Assert.NotNull(pi);

            var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
            Assert.NotNull(queryAttr);
            Assert.True(queryAttr.Converter is BoolConverter);

            queryAttr.Converter.FromUrl(testSource, testClass, pi, queryAttr);

            Assert.AreEqual(expected, testClass.BoolMember);
        }

        [Test]
        public void CanConvert_IsoDate_ToUrl()
        {
            var testClass = new TestClass {DateMember = new DateTime(2015, 10, 10)};
            var expected = "dm=2015-10-10T04:00:00Z";
            
            var pi = testClass.GetType().GetProperties().First(p => p.Name == "DateMember");
            Assert.NotNull(pi);

            var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
            Assert.NotNull(queryAttr);
            Assert.True(queryAttr.Converter is IsoDateConverter);

            var result = queryAttr.Converter.ToUrl(testClass, pi, queryAttr);

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void CanConvert_IsoDate_FromUrl()
        {
            var testClass = new TestClass();
            
            var testSource = new Dictionary<string, string> {{"dm", "2015-10-10T04:00:00Z"}};
            var expected = new DateTime(2015, 10, 10, 0, 0, 0, DateTimeKind.Local);
            
            var pi = testClass.GetType().GetProperties().First(p => p.Name == "DateMember");
            Assert.NotNull(pi);

            var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
            Assert.NotNull(queryAttr);
            Assert.True(queryAttr.Converter is IsoDateConverter);

            queryAttr.Converter.FromUrl(testSource, testClass, pi, queryAttr);

            Assert.AreEqual(expected, testClass.DateMember);
        }
        
        [Test]
        public void IsoDate_ToUrl_ReturnsEmptyStringForDefaultDateTime()
        {
            var testClass = new TestClass();
            var expected = "";
            
            var pi = testClass.GetType().GetProperties().First(p => p.Name == "DateMember");
            Assert.NotNull(pi);

            var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
            Assert.NotNull(queryAttr);
            Assert.True(queryAttr.Converter is IsoDateConverter);

            var result = queryAttr.Converter.ToUrl(testClass, pi, queryAttr);

            Assert.AreEqual(expected, result);
        }


        [TestCase(TestEnum.V0, "em=0")]
        [TestCase(TestEnum.V1, "em=1")]
        [TestCase(TestEnum.V2, "em=2")]
        public void CanConvert_EnumInt_ToUrl(TestEnum value, string expected)
        {
            var testClass = new TestClass {EnumIntMember = value};
            
            var pi = testClass.GetType().GetProperties().First(p => p.Name == "EnumIntMember");
            Assert.NotNull(pi);

            var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
            Assert.NotNull(queryAttr);
            Assert.True(queryAttr.Converter is EnumIntConverter);

            var result = queryAttr.Converter.ToUrl(testClass, pi, queryAttr);

            Assert.AreEqual(expected, result);
        }
        
        [TestCase("em=0", TestEnum.V0)]
        [TestCase("em=1", TestEnum.V1)]
        [TestCase("em=2", TestEnum.V2)]
        public void CanConvert_EnumInt_FromUrl(string value, TestEnum expected)
        {
            var testClass = new TestClass();
            var testSource = new Dictionary<string, string> {{value.Split('=')[0], value.Split('=')[1]}};
            
            var pi = testClass.GetType().GetProperties().First(p => p.Name == "EnumIntMember");
            Assert.NotNull(pi);

            var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
            Assert.NotNull(queryAttr);
            Assert.True(queryAttr.Converter is EnumIntConverter);

            queryAttr.Converter.FromUrl(testSource, testClass, pi, queryAttr);

            Assert.AreEqual(expected, testClass.EnumIntMember);
        }

        [TestCase(TestEnum.V0, "esm=V0")]
        [TestCase(TestEnum.V1, "esm=V1")]
        [TestCase(TestEnum.V2, "esm=V2")]
        public void CanConvert_EnumString_ToUrl(TestEnum value, string expected)
        {
            var testClass = new TestClass {EnumStringMember = value};
            
            var pi = testClass.GetType().GetProperties().First(p => p.Name == "EnumStringMember");
            Assert.NotNull(pi);

            var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
            Assert.NotNull(queryAttr);
            Assert.True(queryAttr.Converter is EnumStringConverter);

            var result = queryAttr.Converter.ToUrl(testClass, pi, queryAttr);

            Assert.AreEqual(expected, result);
        }
        
        [TestCase("esm=V0", TestEnum.V0)]
        [TestCase("esm=V1", TestEnum.V1)]
        [TestCase("esm=V2", TestEnum.V2)]
        public void CanConvert_EnumString_FromUrl(string value, TestEnum expected)
        {
            var testClass = new TestClass();
            var testSource = new Dictionary<string, string> {{value.Split('=')[0], value.Split('=')[1]}};
            
            var pi = testClass.GetType().GetProperties().First(p => p.Name == "EnumStringMember");
            Assert.NotNull(pi);

            var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
            Assert.NotNull(queryAttr);
            Assert.True(queryAttr.Converter is EnumStringConverter);

            queryAttr.Converter.FromUrl(testSource, testClass, pi, queryAttr);

            Assert.AreEqual(expected, testClass.EnumStringMember);
        }


        [TestCase(new int[] {}, "")]
        [TestCase(new int[] {10}, "iam=10")]
        [TestCase(new int[] {2,4,12,-100}, "iam=2;4;12;-100")]
        public void CanConvert_IntArray_ToUrl(int[] value, string expected)
        {
            var testClass = new TestClass {IntArrayMember = value};
            
            var pi = testClass.GetType().GetProperties().First(p => p.Name == "IntArrayMember");
            Assert.NotNull(pi);

            var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
            Assert.NotNull(queryAttr);
            Assert.True(queryAttr.Converter is IntArrayConverter);

            var result = queryAttr.Converter.ToUrl(testClass, pi, queryAttr);

            Assert.AreEqual(expected, result);
        }
        
        [TestCase("", new int[] {})]
        [TestCase("iam=10", new int[] {10})]
        [TestCase("iam=2;4;12;-100", new int[] {2,4,12,-100})]
        public void CanConvert_IntArray_FromUrl(string value, int[] expected)
        {
            var testClass = new TestClass();
            var testSource = new Dictionary<string, string>();
            if(!string.IsNullOrWhiteSpace(value))
                testSource.Add(value.Split('=')[0], value.Split('=')[1]);
            
            var pi = testClass.GetType().GetProperties().First(p => p.Name == "IntArrayMember");
            Assert.NotNull(pi);

            var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
            Assert.NotNull(queryAttr);
            Assert.True(queryAttr.Converter is IntArrayConverter);

            queryAttr.Converter.FromUrl(testSource, testClass, pi, queryAttr);

            Assert.AreEqual(expected, testClass.IntArrayMember);
        }


        [TestCase(new string[] {}, "")]
        [TestCase(new string[] {"10"}, "sam=10")]
        [TestCase(new string[] {"2","4","12","-100"}, "sam=2;4;12;-100")]
        public void CanConvert_StringArray_ToUrl(string[] value, string expected)
        {
            var testClass = new TestClass {StringArrayMember = value};
            
            var pi = testClass.GetType().GetProperties().First(p => p.Name == "StringArrayMember");
            Assert.NotNull(pi);

            var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
            Assert.NotNull(queryAttr);
            Assert.True(queryAttr.Converter is StringArrayConverter);

            var result = queryAttr.Converter.ToUrl(testClass, pi, queryAttr);

            Assert.AreEqual(expected, result);
        }
        
        [TestCase("", new string[] {})]
        [TestCase("sam=10", new string[] {"10"})]
        [TestCase("sam=2;4;12;-100", new string[] {"2","4","12","-100"})]
        public void CanConvert_StringArray_FromUrl(string value, string[] expected)
        {
            var testClass = new TestClass();
            var testSource = new Dictionary<string, string>();
            if(!string.IsNullOrWhiteSpace(value))
                testSource.Add(value.Split('=')[0], value.Split('=')[1]);
            
            var pi = testClass.GetType().GetProperties().First(p => p.Name == "StringArrayMember");
            Assert.NotNull(pi);

            var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
            Assert.NotNull(queryAttr);
            Assert.True(queryAttr.Converter is StringArrayConverter);

            queryAttr.Converter.FromUrl(testSource, testClass, pi, queryAttr);

            Assert.AreEqual(expected, testClass.StringArrayMember);
        }


        [TestCase(new TestEnum[] {}, "")]
        [TestCase(new TestEnum[] {TestEnum.V1}, "eam=1")]
        [TestCase(new TestEnum[] {TestEnum.V0, TestEnum.V2, TestEnum.V1}, "eam=0;2;1")]
        public void CanConvert_EnumIntArray_ToUrl(TestEnum[] value, string expected)
        {
            var testClass = new TestClass {EnumIntArrayMember = value};
            
            var pi = testClass.GetType().GetProperties().First(p => p.Name == "EnumIntArrayMember");
            Assert.NotNull(pi);

            var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
            Assert.NotNull(queryAttr);
            Assert.True(queryAttr.Converter is EnumIntArrayConverter);

            var result = queryAttr.Converter.ToUrl(testClass, pi, queryAttr);

            Assert.AreEqual(expected, result);
        }


        [TestCase("", new TestEnum[] {})]
        [TestCase("eam=1", new TestEnum[] {TestEnum.V1})]
        [TestCase("eam=2;0;1", new TestEnum[] {TestEnum.V2, TestEnum.V0, TestEnum.V1})]
        public void CanConvert_EnumIntArray_FromUrl(string value, int[] expected)
        {
            var testClass = new TestClass();
            var testSource = new Dictionary<string, string>();
            if(!string.IsNullOrWhiteSpace(value))
                testSource.Add(value.Split('=')[0], value.Split('=')[1]);
            
            var pi = testClass.GetType().GetProperties().First(p => p.Name == "EnumIntArrayMember");
            Assert.NotNull(pi);

            var queryAttr = pi.GetCustomAttributes(true).FirstOrDefault(a => a is UrlQueryParamAttribute) as UrlQueryParamAttribute;
            Assert.NotNull(queryAttr);
            Assert.True(queryAttr.Converter is EnumIntArrayConverter);

            queryAttr.Converter.FromUrl(testSource, testClass, pi, queryAttr);

            Assert.AreEqual(expected, testClass.EnumIntArrayMember);
        }

        public enum TestEnum
        {
            V0,
            V1,
            V2
        }

        public class TestClass
        {
            [UrlQueryParam(typeof(StringConverter), "sm")]
            public string StringMember { get; set; }
            
            [UrlQueryParam(typeof(Int32Converter), "im")]
            public int Int32Member { get; set; }
            
            [UrlQueryParam(typeof(BoolConverter), "bm")]
            public bool BoolMember { get; set; }
            
            [UrlQueryParam(typeof(IsoDateConverter), "dm")]
            public DateTime DateMember { get; set; }
            
            [UrlQueryParam(typeof(EnumIntConverter), "em")]
            public TestEnum EnumIntMember { get; set; }
            
            [UrlQueryParam(typeof(EnumStringConverter), "esm")]
            public TestEnum EnumStringMember { get; set; }

            [UrlQueryParam(typeof(IntArrayConverter), "iam")]
            public int[] IntArrayMember { get; set; }
            
            [UrlQueryParam(typeof(StringArrayConverter), "sam")]
            public string[] StringArrayMember { get; set; }

            [UrlQueryParam(typeof(EnumIntArrayConverter), "eam")]
            public TestEnum[] EnumIntArrayMember { get; set; }
        }
    }
}