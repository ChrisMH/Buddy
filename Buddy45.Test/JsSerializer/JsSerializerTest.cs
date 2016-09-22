using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NUnit.Framework;
using Buddy.JsSerializer;

namespace Buddy.Test.JsSerializer
{
    public class JsSerializerTest
    {
        [Test, TestCaseSource(typeof(JsSerializerTestData), nameof(JsSerializerTestData.SerializesSimpleTypeCases))]
        public void SerializesSimpleType(object serialize, string expected)
        {
            ExecuteTest(serialize, expected);
        }

        [Test, TestCaseSource(typeof(JsSerializerTestData), nameof(JsSerializerTestData.SerializesClassCases))]
        public void SerializesClass(object serialize, string expected)
        {
            ExecuteTest(serialize, expected);
        }

        [Test, TestCaseSource(typeof(JsSerializerTestData), nameof(JsSerializerTestData.SerializesListCases))]
        public void SerializesList(object serialize, string expected)
        {
            ExecuteTest(serialize, expected);
        }

        [Test, TestCaseSource(typeof(JsSerializerTestData), nameof(JsSerializerTestData.SerializesDictCases))]
        public void SerializedDict(object serialize, string expected)
        {
            ExecuteTest(serialize, expected);
        }
        
        private void ExecuteTest(object serialize, string expected)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                var serializer = new Buddy.JsSerializer.JsSerializer();
                serializer.Serialize(sw, serialize);
            }
            Console.WriteLine("Expected:");
            Console.WriteLine(expected);
            Console.WriteLine("Actual:");
            Console.WriteLine(sb.ToString());

            Assert.AreEqual(expected, sb.ToString());
        }
    }


    public class JsSerializerTestData
    {
        private static readonly SimpleTypes SimpleTypes1 = new SimpleTypes
        {
            StringProp = "One",
            BoolProp = true,
            LongProp = long.MaxValue,
            ULongProp = ulong.MaxValue,
            IntProp = int.MaxValue,
            UIntProp = uint.MaxValue,
            ShortProp = short.MaxValue,
            UShortProp = ushort.MaxValue,
            ByteProp = byte.MaxValue,
            SByteProp = sbyte.MaxValue,
            DoubleProp = double.MaxValue,
            FloatProp = float.MaxValue
        };
        private static readonly SimpleTypes SimpleTypes2 = new SimpleTypes
        {
            StringProp = "Two",
            BoolProp = false,
            LongProp = long.MinValue,
            ULongProp = ulong.MinValue,
            IntProp = int.MinValue,
            UIntProp = uint.MinValue,
            ShortProp = short.MinValue,
            UShortProp = ushort.MinValue,
            ByteProp = byte.MinValue,
            SByteProp = sbyte.MinValue,
            DoubleProp = double.MinValue,
            FloatProp = float.MinValue
        };
        private static readonly SimpleTypes SimpleTypes3 = new SimpleTypes
        {
            StringProp = "Three",
            BoolProp = true,
            LongProp = 0,
            ULongProp = 0,
            IntProp = 0,
            UIntProp = 0,
            ShortProp = 0,
            UShortProp = 0,
            ByteProp = 0,
            SByteProp = 0,
            DoubleProp = 0,
            FloatProp = 0
        };

        private static readonly ClassTypes ClassTypes1 = new ClassTypes
        {
            Class1Prop = SimpleTypes1,
            Class2Prop = SimpleTypes2
        };
        private static readonly ClassTypes ClassTypes2 = new ClassTypes
        {
            Class1Prop = SimpleTypes1,
            Class2Prop = null
        };

        private static readonly List<SimpleTypes> List1 = new List<SimpleTypes> {SimpleTypes1, SimpleTypes2};
        private static readonly string List1Expected = $"[{SimpleTypes1.ExpectedJavascript},{SimpleTypes2.ExpectedJavascript}]";

        private static readonly SimpleTypes[] List2 = { SimpleTypes1, SimpleTypes2 };
        private static readonly string List2Expected = $"[{SimpleTypes1.ExpectedJavascript},{SimpleTypes2.ExpectedJavascript}]";

        private static readonly SimpleTypes[] List3 = { SimpleTypes1, null };
        private static readonly string List3Expected = $"[{SimpleTypes1.ExpectedJavascript},null]";
        private static readonly int[] List4 = { 5, 4, 3, 2, 1 };
        private static readonly string List4Expected = "[5,4,3,2,1]";
        private static readonly string[] List5 = { "5", "4", "3", "2", "1" };
        private static readonly string List5Expected = "[\"5\",\"4\",\"3\",\"2\",\"1\"]";

        private static readonly Dictionary<int, string> Dict1 = new Dictionary<int, string>
        {
            {1, "One"},
            {2, "Two"},
            {3, "Three"}
        };

        private static readonly string Dict1Expected = "{1:\"One\",2:\"Two\",3:\"Three\"}";

        private static readonly Dictionary<int, SimpleTypes> Dict2 = new Dictionary<int, SimpleTypes>
        {
            {1, SimpleTypes1},
            {2, SimpleTypes2},
            {3, SimpleTypes3}
        };
        private static readonly string Dict2Expected = $"{{1:{SimpleTypes1.ExpectedJavascript},2:{SimpleTypes2.ExpectedJavascript},3:{SimpleTypes3.ExpectedJavascript}}}";

        private static readonly Dictionary<int, SimpleTypes> Dict3 = new Dictionary<int, SimpleTypes>
        {
            {1, SimpleTypes1},
            {2, null},
            {3, SimpleTypes3}
        };

        private static readonly string Dict3Expected = $"{{1:{SimpleTypes1.ExpectedJavascript},2:null,3:{SimpleTypes3.ExpectedJavascript}}}";


        private static readonly MixedTypes MixedTypes1 = new MixedTypes
        {
            StringProp = null,
            ClassProp = null,
            ListProp = null,
            DictProp = null
        };

        private static readonly string MixedTypes1Expected =
            "{" +
            "stringProp:null," +
            "classProp:null," +
            "listProp:null," +
            "dictProp:null" +
            "}";
        private static readonly MixedTypes MixedTypes2 = new MixedTypes
        {
            StringProp = "String",
            ClassProp = SimpleTypes1,
            ListProp = List1,
            DictProp = Dict2
        };

        private static readonly string MixedTypes2Expected =
            "{" +
            "stringProp:\"String\"," +
            $"classProp:{SimpleTypes1.ExpectedJavascript}," +
            $"listProp:{List1Expected}," +
            $"dictProp:{Dict2Expected}" +
            "}";


        public static IEnumerable SerializesSimpleTypeCases
        {
            get
            {
                yield return new TestCaseData(1, "1");
                yield return new TestCaseData(null, "null");
                yield return new TestCaseData("One", "\"One\"");
            }
        }

        public static IEnumerable SerializesClassCases
        {
            get
            {
                yield return new TestCaseData(SimpleTypes1, SimpleTypes1.ExpectedJavascript);
                yield return new TestCaseData(SimpleTypes2, SimpleTypes2.ExpectedJavascript);
                yield return new TestCaseData(SimpleTypes3, SimpleTypes3.ExpectedJavascript);
                yield return new TestCaseData(ClassTypes1, ClassTypes1.ExpectedJavascript);
                yield return new TestCaseData(ClassTypes2, ClassTypes2.ExpectedJavascript);
                yield return new TestCaseData(MixedTypes1, MixedTypes1Expected);
                yield return new TestCaseData(MixedTypes2, MixedTypes2Expected);
            }
        }

        public static IEnumerable SerializesListCases
        {
            get
            {
                yield return new TestCaseData(List1, List1Expected);
                yield return new TestCaseData(List2, List2Expected);
                yield return new TestCaseData(List3, List3Expected);
                yield return new TestCaseData(List4, List4Expected);
                yield return new TestCaseData(List5, List5Expected);
            }
        }

        public static IEnumerable SerializesDictCases
        {
            get
            {
                yield return new TestCaseData(Dict1, Dict1Expected);
                yield return new TestCaseData(Dict2, Dict2Expected);
                yield return new TestCaseData(Dict3, Dict3Expected);
            }
        }
    }



    public interface ISerializerTestType
    {
        string ExpectedJavascript { get; }
    }

    public class SimpleTypes : ISerializerTestType
    {
        public string StringProp { get; set; }
        public bool BoolProp { get; set; }
        public long LongProp { get; set; }
        public ulong ULongProp { get; set; }
        public int IntProp { get; set; }
        public uint UIntProp { get; set; }
        public short ShortProp { get; set; }
        public ushort UShortProp { get; set; }
        public byte ByteProp { get; set; }
        public sbyte SByteProp { get; set; }
        public double DoubleProp { get; set; }
        public float FloatProp { get; set; }

        [JsIgnore]
        public string ExpectedJavascript
        {
            get
            {
                var strStringProp = StringProp == null ? "null" : $"\"{StringProp}\"";
                return "{" +
                       $"stringProp:{strStringProp}," +
                       $"boolProp:{BoolProp.ToString().ToLower()}," +
                       $"longProp:{LongProp}," +
                       $"uLongProp:{ULongProp}," +
                       $"intProp:{IntProp}," +
                       $"uIntProp:{UIntProp}," +
                       $"shortProp:{ShortProp}," +
                       $"uShortProp:{UShortProp}," +
                       $"byteProp:{ByteProp}," +
                       $"sByteProp:{SByteProp}," +
                       $"doubleProp:{DoubleProp}," +
                       $"floatProp:{FloatProp}" +
                       "}";
            }
        }
    }
    public class ClassTypes : ISerializerTestType
    {
        public SimpleTypes Class1Prop { get; set; }
        public SimpleTypes Class2Prop { get; set; }

        [JsIgnore]
        public string ExpectedJavascript
        {
            get
            {
                var strClass1Prop = Class1Prop != null ? Class1Prop.ExpectedJavascript : "null";
                var strClass2Prop = Class2Prop != null ? Class2Prop.ExpectedJavascript : "null";
                return "{" +
                       $"class1Prop:{strClass1Prop}," +
                       $"class2Prop:{strClass2Prop}" +
                       "}";
            }
        }
    }

    public class MixedTypes
    {
        public string StringProp { get; set; }
        public SimpleTypes ClassProp { get; set; }
        public List<SimpleTypes> ListProp { get; set; }
        public Dictionary<int,SimpleTypes> DictProp  { get; set; }
    }
}