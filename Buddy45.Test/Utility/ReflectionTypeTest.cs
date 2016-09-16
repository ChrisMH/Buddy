using System;
using Buddy.Utility;
using NUnit.Framework;

namespace Buddy.Test.Utility
{
    public class ReflectionTypeTest
    {
        [TestCase("System.Data.SqlClient.SqlClientFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
          "System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
          "System.Data.SqlClient.SqlClientFactory")]
        [TestCase("Npgsql.NpgsqlFactory, Npgsql", "Npgsql", "Npgsql.NpgsqlFactory")]
        [TestCase("Buddy.Test.Utility.TestType", null, "Buddy.Test.Utility.TestType")]
        public void CanCreateFromFullName(string fullName, string assemblyName, string className)
        {
            var result = new ReflectionType(fullName);

            Assert.AreEqual(assemblyName, result.AssemblyName);
            Assert.AreEqual(className, result.ClassName);
        }

        [TestCase(typeof(System.Data.SqlClient.SqlClientFactory),
          "System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
          "System.Data.SqlClient.SqlClientFactory")]
        [TestCase(typeof(TestType), "Buddy.Test, ", "Buddy.Test.Utility.TestType")]
        public void CanCreateFromType(Type type, string assemblyName, string className)
        {
            var result = new ReflectionType(type);

            Assert.That(result.AssemblyName.StartsWith(assemblyName));
            Assert.AreEqual(className, result.ClassName);
        }

        [TestCase("")]
        [TestCase("    ")]
        public void CreateThrowsWhenFullNameIsInvalid(string fullName)
        {
            var e = Assert.Throws<ArgumentException>(() => new ReflectionType(fullName));
            Assert.AreEqual("fullName", e.ParamName);
            Console.WriteLine(e.Message);
        }

        [TestCase("System.Data.SqlClient.SqlClientFactory, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089",
          typeof(System.Data.SqlClient.SqlClientFactory))]
        [TestCase("Buddy.Test.Utility.TestType", typeof(TestType))]
        public void CanCreateType(string fullName, Type expectedType)
        {
            var type = new ReflectionType(fullName);
            var result = type.CreateType();

            Assert.AreEqual(expectedType, result);
        }

        [TestCase("TestType")]
        [TestCase("System.Data.SqlClient.SqlClientFactory, System.Data")]
        [TestCase("System.Data.SqlClient.BadTypeName, System.Data, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
        public void CreateTypeThrowsWhenTypeCannotBeCreated(string fullName)
        {
            var type = new ReflectionType(fullName);
            var e = Assert.Throws<ApplicationException>(() => type.CreateType());
            Console.WriteLine(e.Message);
        }

        [Test]
        public void CanCreateInstanceWith0Parameters()
        {
            var type = new ReflectionType("Buddy.Test.Utility.TestType");
            var result = type.CreateObject();

            Assert.IsInstanceOf(typeof(TestType), result);
            Assert.IsNull(((TestType)result).P1);
            Assert.IsNull(((TestType)result).P2);
        }

        [Test]
        public void CanCreateInstanceWith1Parameter()
        {
            var type = new ReflectionType("Buddy.Test.Utility.TestType");
            var result = type.CreateObject("Parameter1");

            Assert.IsInstanceOf(typeof(TestType), result);
            Assert.AreEqual("Parameter1", ((TestType)result).P1);
            Assert.IsNull(((TestType)result).P2);
        }

        [Test]
        public void CanCreateInstanceWith2Parameters()
        {
            var type = new ReflectionType("Buddy.Test.Utility.TestType");
            var result = type.CreateObject("Parameter1", "Parameter2");

            Assert.IsInstanceOf(typeof(TestType), result);
            Assert.AreEqual("Parameter1", ((TestType)result).P1);
            Assert.AreEqual("Parameter2", ((TestType)result).P2);
        }

        [Test]
        public void CanCreateInstanceReturningSpecificType()
        {
            var type = new ReflectionType("Buddy.Test.Utility.TestType");
            TestType result = type.CreateObject<TestType>();

            Assert.That(result, Is.InstanceOf<TestType>());
        }

    }
}