using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utility
{
  /// <summary>
  ///   Extension methods for collection classes
  /// </summary>
  public static class CollectionExtensions
  {
    /// <summary>
    ///   Performs an action for each element in a collection
    /// </summary>
    /// <typeparam name = "T">Type of element in the collection</typeparam>
    /// <param name = "coll">The collection</param>
    /// <param name = "action">The action to perform</param>
    public static void ForEach<T>(this IEnumerable<T> coll, Action<T> action)
    {
      foreach (var c in coll)
      {
        action.Invoke(c);
      }
    }

    /*
    public static string Join<T>(this IEnumerable<T> coll, Func<T, string> transformElement, string separator = null)
    {
      var result = coll.Aggregate("", (current, c) => current + (transformElement.Invoke(c) + (separator ?? "")));

      if (separator != null && !string.IsNullOrEmpty(separator))
      {
        result = result.Remove(result.Length - separator.Length, separator.Length);
      }

      return result;
    }
    */

    /// <summary>
    ///   Projects each element of an enumerator into a new form
    /// </summary>
    /// <typeparam name = "T">Type of element in the enumerator</typeparam>
    /// <typeparam name = "TResult">Resulting type of element in the projected enumeration</typeparam>
    /// <param name = "enumerator"></param>
    /// <param name = "selector"></param>
    /// <returns></returns>
    public static IEnumerable<TResult> Select<T, TResult>(this IEnumerator enumerator, Func<T, TResult> selector)
    {
      while (enumerator.MoveNext())
      {
        yield return selector.Invoke((T) enumerator.Current);
      }
    }

    /// <summary>
    ///   Filters an enumerator based on a predicate.
    /// </summary>
    /// <typeparam name = "T">Type of element in the enumerator</typeparam>
    /// <param name = "enumerator"></param>
    /// <param name = "predicate"></param>
    /// <returns></returns>
    public static IEnumerable<T> Where<T>(this IEnumerator enumerator, Func<T, bool> predicate)
    {
      while (enumerator.MoveNext())
      {
        if (predicate.Invoke((T) enumerator.Current))
        {
          yield return (T) enumerator.Current;
        }
      }
    }

    /// <summary>
    ///   Performs an action for each element in an enumerator collection
    /// </summary>
    /// <typeparam name = "T">Type of element in the collection</typeparam>
    /// <param name = "enumerator"></param>
    /// <param name = "action">The action to perform</param>
    public static void ForEach<T>(this IEnumerator enumerator, Action<T> action)
    {
      while (enumerator.MoveNext())
      {
        action.Invoke((T) enumerator.Current);
      }
    }
  }
}