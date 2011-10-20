using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Utility.Test
{
  public class ReflectionTypeTest
  {
    [Test]
    public void CanCreateFromPackedString()
    {
      var type = new ReflectionType( "TheAssembly:TheAssembly.TheNamespace.TheType" );

      Assert.AreEqual( "TheAssembly", type.AssemblyName );
      Assert.AreEqual( "TheAssembly.TheNamespace.TheType", type.ClassName );
      
    }

  }
}
