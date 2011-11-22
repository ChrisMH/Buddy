using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Utility
{
  public static class Path
  {
    /// <summary>
    /// Removes elements of a file path according to special strings inserted into that path.
    /// 
    /// Wraps the shell PathCanonicalize function
    /// </summary>
    /// <param name="src">Path to canonicalize</param>
    /// <returns></returns>
    public static string Canonicalize(string src)
    {
      var sb = new StringBuilder(2048);
      PathCanonicalize(sb, src);
      return sb.ToString();
    }

    /// <summary>
    /// Concatenates two strings that represent properly formed paths into one path; also concatenates any relative path elements.
    /// 
    /// Wraps the shell PathCombine function
    /// </summary>
    /// <param name="src1">first part of the path to combine</param>
    /// <param name="src2">second part of the path to combine</param>
    /// <returns></returns>
    public static string Combine(string src1, string src2)
    {
      var sb = new StringBuilder(2048);
      PathCombine(sb, src1, src2);
      return sb.ToString();
    }

    [DllImport("shlwapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern bool PathCanonicalize([Out] StringBuilder dst, string src);
    
    [DllImport("shlwapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr PathCombine([Out] StringBuilder lpszDest, string lpszDir, string lpszFile);
  }
}