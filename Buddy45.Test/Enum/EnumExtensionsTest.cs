using Buddy.Enum;
using NUnit.Framework;

namespace Buddy45.Test.Enum
{
    public enum TestEnumType
    {
        [System.ComponentModel.Description("zero")]
        Zero,
        
        [System.ComponentModel.Description("one")]
        One,

        Two,
        
        [System.ComponentModel.Description("three")]
        Three
    }

    public class EnumExtensionsTest
    {
        [TestCase(TestEnumType.Zero, "zero")]
        [TestCase(TestEnumType.One, "one")]
        [TestCase(TestEnumType.Three, "three")]
        public void GetDescription_ReturnsDescription(TestEnumType value, string expected)
        {
            Assert.AreEqual(expected, value.GetDescription());
        }

        [TestCase(TestEnumType.Two)]
        public void GetDescription_ReturnsEmptyIfNoDescription(TestEnumType value)
        {
            Assert.AreEqual(string.Empty, value.GetDescription());
        }

        [TestCase("zero", TestEnumType.Zero, TestEnumType.One)]
        [TestCase("one", TestEnumType.One, TestEnumType.Zero)]
        [TestCase("three", TestEnumType.Three, TestEnumType.Zero)]
        public void GetEnumValueFromDescription_ReturnsValue(string description, TestEnumType expected, TestEnumType defaultValue)
        {
            Assert.AreEqual(expected, description.GetEnumValueFromDescription(defaultValue));
        }

        [TestCase("two")]
        public void GetEnumValueFromDescription_ReturnsDefaultValueIfDescriptionNotFound(string description)
        {
            Assert.AreEqual(TestEnumType.Zero, description.GetEnumValueFromDescription(TestEnumType.Zero));
        }

    }
}
