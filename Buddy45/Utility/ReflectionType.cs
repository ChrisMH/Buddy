using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Buddy.Utility
{
    [Serializable]
    public class ReflectionType : IComparable<Type>
    {
        public ReflectionType(string fullName)
        {
            var match = Regex.Match(fullName, @"(?<ClassName>^[^,\s]*)[,\s]*(?<AssemblyName>.*$)");
            if (!match.Success)
            {
                throw new ArgumentException(string.Format("Invalid fullName: '{0}'", fullName), "fullName");
            }
            AssemblyName = string.IsNullOrWhiteSpace(match.Groups["AssemblyName"].Value) ? null : match.Groups["AssemblyName"].Value;
            ClassName = match.Groups["ClassName"].Value;

            if (string.IsNullOrWhiteSpace(ClassName))
            {
                throw new ArgumentException(string.Format("Invalid fullName: '{0}'", fullName), "fullName");
            }
        }

        public ReflectionType(Type type)
            : this(type.AssemblyQualifiedName)
        {
        }

        public string AssemblyName { get; private set; }
        public string ClassName { get; private set; }

        public object CreateObject(params object[] parameters)
        {
            return CreateType().Assembly.CreateInstance(ClassName, false, BindingFlags.CreateInstance,
                null, parameters, null, null);
        }

        public T CreateObject<T>(params object[] parameters)
        {
            return (T) CreateType().Assembly.CreateInstance(ClassName, false, BindingFlags.CreateInstance,
                null, parameters, null, null);
        }

        public Type CreateType()
        {
            if (classType != null)
            {
                return classType;
            }

            if (AssemblyName == null)
            {
                classType = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName.Equals(ClassName));

                if (classType == null)
                {
                    throw new ApplicationException(string.Format("Could not find a type matching '{0}' in any loaded assemblies", ClassName));
                }
            }
            else
            {
                Assembly assembly;
                try
                {
                    assembly = Assembly.Load(AssemblyName);
                }
                catch (Exception e)
                {
                    throw new ApplicationException(string.Format("Unable to load assembly named '{0}'", AssemblyName), e);
                }

                classType = assembly.GetType(ClassName, false, false);
                if (classType == null)
                {
                    throw new ApplicationException(string.Format("Type '{0}' is not defined in assembly '{1}'", ClassName, AssemblyName));
                }
            }
            return classType;
        }

        public int CompareTo(Type other)
        {
            return CreateType().AssemblyQualifiedName.CompareTo(other.AssemblyQualifiedName);
        }

        public override string ToString()
        {
            return CreateType().AssemblyQualifiedName;
        }

        private Type classType;
    }
}