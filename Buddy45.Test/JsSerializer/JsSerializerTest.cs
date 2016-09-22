using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using Buddy.JsSerializer;

namespace Buddy.Test.JsSerializer
{
    public class JsSerializerTest
    {
        [Test]
        public void SerializesClass()
        {
            var serialize = new SimpleClass { StringProp = "1", IntProp = 1, BoolProp = true };
            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                var serializer = new Buddy.JsSerializer.JsSerializer();
                serializer.Serialize(sw, serialize);
            }

            var expected = "{stringProp:\"1\",intProp:1,boolProp:true}";
            Assert.AreEqual(expected, sb.ToString());
        }

        [Test]
        public void SerializesClassComposedOfClasses()
        {
            var serialize = new ParentClass
            {
                StringProp = "1",
                IntProp = 1,
                BoolProp = true,
                SimpleClass1 = new SimpleClass { StringProp = "2", IntProp = 2, BoolProp = false },
                SimpleClass2 = new SimpleClass { StringProp = "3", IntProp = 3, BoolProp = true }
            };

            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                var serializer = new Buddy.JsSerializer.JsSerializer();
                serializer.Serialize(sw, serialize);
            }

            var expected = "{stringProp:\"1\",intProp:1,boolProp:true," +
                           "simpleClass1:{stringProp:\"2\",intProp:2,boolProp:false}," +
                           "simpleClass2:{stringProp:\"3\",intProp:3,boolProp:true}}";
            Assert.AreEqual(expected, sb.ToString());
        }


        [Test]
        public void SerializesClassComposedOfLists()
        {
            var serialize = new ListClass
            {
                StringProp = "1",
                IntProp = 1,
                BoolProp = true,
                SimpleList1 = new List<SimpleClass>
                    { new SimpleClass { StringProp = "2", IntProp = 2, BoolProp = false }, new SimpleClass { StringProp = "3", IntProp = 3, BoolProp = true } },
                SimpleList2 = new SimpleClass[2]
                    { new SimpleClass { StringProp = "4", IntProp = 4, BoolProp = false }, new SimpleClass { StringProp = "5", IntProp = 5, BoolProp = true } }
            };

            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                var serializer = new Buddy.JsSerializer.JsSerializer();
                serializer.Serialize(sw, serialize);
            }

            Console.WriteLine(sb.ToString());
            var expected = "{stringProp:\"1\",intProp:1,boolProp:true," +
                           "simpleList1:[{stringProp:\"2\",intProp:2,boolProp:false},{stringProp:\"3\",intProp:3,boolProp:true}]," +
                           "simpleList2:[{stringProp:\"4\",intProp:4,boolProp:false},{stringProp:\"5\",intProp:5,boolProp:true}]}";
            Assert.AreEqual(expected, sb.ToString());
        }

        [Test]
        public void SerializesClassComposedOfDictionaries()
        {
            var serialize = new DictionaryClass
            {
                StringProp = "1",
                IntProp = 1,
                BoolProp = true,
                SimpleDict1 = new Dictionary<string, SimpleClass> {
                    { "One", new SimpleClass { StringProp = "2", IntProp = 2, BoolProp = false } },
                    { "Two", new SimpleClass { StringProp = "4", IntProp = 4, BoolProp = false } } }
            };

            var sb = new StringBuilder();

            using (var sw = new StringWriter(sb))
            {
                var serializer = new Buddy.JsSerializer.JsSerializer();
                serializer.Serialize(sw, serialize);
            }

            Console.WriteLine(sb.ToString());
            var expected = "{stringProp:\"1\",intProp:1,boolProp:true," +
                           "simpleDict1:{\"One\":{stringProp:\"2\",intProp:2,boolProp:false},\"Two\":{stringProp:\"4\",intProp:4,boolProp:false}}}";
            Assert.AreEqual(expected, sb.ToString());
        }



        internal class ListClass
        {
            public string StringProp { get; set; }
            public int IntProp { get; set; }
            public bool BoolProp { get; set; }

            public List<SimpleClass> SimpleList1 {get; set; }
            public SimpleClass[] SimpleList2 { get; set; }

        }

        internal class DictionaryClass
        {
            public string StringProp { get; set; }
            public int IntProp { get; set; }
            public bool BoolProp { get; set; }

            public Dictionary<string, SimpleClass> SimpleDict1 { get; set; }

        }

        internal class ParentClass
        {
            public string StringProp { get; set; }
            public int IntProp { get; set; }
            public bool BoolProp { get; set; }

            public SimpleClass SimpleClass1 { get; set; }
            public SimpleClass SimpleClass2 { get; set; }
        }

        internal class SimpleClass
        {
            public string StringProp { get; set; }
            public int IntProp { get; set; }
            public bool BoolProp { get; set; }
        }
    }
}