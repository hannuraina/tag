/**
 * File: StringExtensions.cs
 * Author: James McClure
 * 
 * Implements string extension methods.
 * 
 * Revision History:
 * 0.2 2011-06-01
 * Added string replacement with overloaded
 * comparison
 * Initial creation.
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
  /// <summary>
  /// Contains extensions for the base class string.
  /// </summary>
  static public class StringExtensions
  {

  #region Static

    /// <summary>
    /// Overloads base string replace to allow comparisons. 
    /// </summary>
    /// <remarks>
    /// Primarily used for case-insensitive replacement.
    /// </remarks>
    /// <see cref="http://bit.ly/aHApoi"/>
    static public string Replace(this string str, string oldValue, string newValue, StringComparison comparison)
    {
      // Stringbuilder much faster than base string operations
      StringBuilder sb = new StringBuilder();

      // Begin search for old value
      int previousIndex = 0;
      int index = str.IndexOf(oldValue, comparison);
      while (index != -1)
      {
        // Match found replace with new character
        sb.Append(str.Substring(previousIndex, index - previousIndex));
        sb.Append(newValue);
        index += oldValue.Length;

        // Apply search comparison
        previousIndex = index;
        index = str.IndexOf(oldValue, index, comparison);
      }

      // Append remainder of string
      sb.Append(str.Substring(previousIndex));
      return sb.ToString();
    }

  #endregion
  
  }
}
