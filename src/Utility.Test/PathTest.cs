using NUnit.Framework;

namespace Utility.Test
{
  public class PathTest
  {
    [Test]
    public void PathCanonicalize()
    {
      var result = Path.Canonicalize(@"C:\Windows\System32\.\Drivers\..");

      Assert.AreEqual(@"C:\Windows\System32", result);

    } 

    [Test]
    public void PathCombine()
    {
      var result = Path.Combine(@"D:\DevL\Overview\Client\src\Client.Test\bin\Debug", @"..\..\..\Client.Web");

      Assert.AreEqual(@"D:\DevL\Overview\Client\src\Client.Web", result);
    }

    [Test]
    public void PathIsRelative()
    {
      Assert.IsFalse(Path.IsRelative("C:\\Program Files"));
      Assert.IsTrue(Path.IsRelative("..\\relative"));
      Assert.IsTrue(Path.IsRelative("relative"));
      Assert.IsTrue(Path.IsRelative("relative.txt"));
    }

    [Test]
    public void PathIsRoot()
    {
      Assert.IsTrue(Path.IsRoot("C:\\"));
      Assert.IsFalse(Path.IsRoot("..\\relative"));
    }
  }
}