using System;
using System.Reflection;

namespace Utility
{
  [Serializable]
  public class ReflectionType : IComparable<Type>
  {
    public ReflectionType(string packed)
    {
      var assemblyClass = packed.Split(new [] {':'});
      if (assemblyClass.Length != 2)
      {
        throw new Exception(String.Format("Invalid packed assembly name/class name string: {0}", packed));
      }
      AssemblyName = assemblyClass[0];
      ClassName = assemblyClass[1];
    }

    public ReflectionType(string assemblyName, string className)
    {
      AssemblyName = assemblyName;
      ClassName = className;
    }

    public ReflectionType(Type type)
    {
      AssemblyName = type.Assembly.GetName().Name;
      ClassName = type.FullName;
      classType = type;
    }

    public string AssemblyName { get; private set; }

    public string ClassName { get; private set; }

    public object CreateObject(object[] parameters)
    {
      return CreateType().Assembly.CreateInstance(ClassName, false, BindingFlags.CreateInstance,
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
        var assembly = Assembly.GetExecutingAssembly();
        classType = assembly.GetType(ClassName, false, false);
        if (classType == null)
        {
          assembly = Assembly.GetCallingAssembly();
          classType = assembly.GetType(ClassName, false, false);
          if (classType == null)
          {
            assembly = Assembly.GetEntryAssembly();
            classType = assembly.GetType(ClassName, false, false);
            if (classType == null)
            {
              throw new Exception(String.Format("Unable to find an assembly containing the type '{0}'",
                                                ClassName));
            }
          }
        }
      }
      else
      {
        Assembly assembly;
        try
        {
          assembly = Assembly.Load(AssemblyName);
        }
        catch (Exception)
        {
          throw new Exception(String.Format("Unable to load assembly named '{0}'", AssemblyName));
        }

        classType = assembly.GetType(ClassName, false, false);
        if (classType == null)
        {
          throw new Exception(String.Format("Type '{0}' is not defined in assembly '{1}'", ClassName,
                                                   AssemblyName));
        }
      }
      return classType;
    }

    public int CompareTo(Type compareTo)
    {
      return ToString().CompareTo(compareTo.ToString());
    }

    public override string ToString()
    {
      return AssemblyName + ":" + ClassName;
    }

    private Type classType;
  }
}