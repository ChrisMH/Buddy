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
      var result = Path.Combine(@"C:\Windows\System32\.\", @"Drivers\..");

      Assert.AreEqual(@"C:\Windows\System32", result);
    }
  }
}