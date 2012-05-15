/**
 * File: CustomFormatter.cs
 * Author: James McClure
 * 
 * Implements IFormatter class. Allows user to
 * combine multiple formatting patterns to a single
 * object.
 * 
 * Revision History:
 * 0.3 2011-06-14 
 * Merged existing formatters. Eliminated
 * formatting interface.
 * 
 * 0.2 2011-05-30 
 * Cleaned up comments
 * 
 * 0.1 2011-05-27 
 * Initial creation
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Common
{

  #region Enum

  /// <summary>
  /// Enumeration for string casing
  /// </summary>
  public enum Cases
  {
    PASCAL=0,
    LOWER,
    UPPER,
  };

  #endregion

  /// <summary>
  /// Provides user with the ability
  /// format Music objects.
  /// </summary>
  public class Formatter
  {
    #region Attributes
    
    /// <summary>
    /// Gets and sets char mapping to be replaced.
    /// </summary>
    private Dictionary<string, string> replacements;

    #endregion

    #region Properties

    /// <summary>
    /// Sets and gets listing of IFormatter objects.
    /// </summary>
    public List<Formatter> Formatters { get; set; }

    /// <summary>
    /// Gets and sets casing for various file
    /// objects.
    /// </summary>
    public Cases CurrentCase { get; set; }
    public Cases AlbumCasing { get;  set; }
    public Cases SongCasing { get; set; }
    public Cases FlatCasing { get; set; }
    public Cases MetadataCasing { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Creates new custom formatter object with specified 
    /// case type.
    /// </summary>
    /// <param name="casing">
    /// Specified which casing format the rename shoul follow
    /// Default: Pascal (Sentence case)
    /// </param>
    public Formatter()
    {
      replacements = new Dictionary<string, string> { 
        { "\\", " "},
        { "/" , " "},
        { ":" , " "},
        { "*" , "" },
        { "?" , "" },
        { "<" , "" },
        { ">" , "" }
      };

      // Default all casing to PASCAL
      CurrentCase = Cases.PASCAL;
      AlbumCasing = Cases.PASCAL;
      SongCasing = Cases.PASCAL;
      FlatCasing = Cases.PASCAL;
      MetadataCasing = Cases.PASCAL;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Applied formatting rules stored in <see cref="Formatter.Formatters"/>
    /// on the specified string.
    /// </summary>
    /// <remarks>
    /// FileTypes isnullable here because Metadata types
    /// do not inherit from the File class. Implicitly 
    /// null filetypes here represent Metadata formatting.
    /// </remarks>
    public string Format(FileTypes? filetype, string value)
    {
      // Determine case formatting
      switch (filetype)
      {
        case FileTypes.RELEASE:
          CurrentCase = AlbumCasing; break;
        case FileTypes.SONG:
          CurrentCase = SongCasing; break;
        case FileTypes.FLAT:
          CurrentCase = FlatCasing; break;
        default:
          CurrentCase = MetadataCasing; break;
      }

      foreach (KeyValuePair<string, string> kv in replacements)
      {
        value = value.Replace(kv.Key, kv.Value, StringComparison.InvariantCultureIgnoreCase);
      }

      value = ApplyCase(value);
      return value;
    }

    /// <summary>
    /// Helper method to append replacements
    /// to replacement dictionary.
    /// </summary>
    /// <param name="set">Replacement set</param>
    public void Add(string key, string value)
    {
      replacements.Add(key, value);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Applies stored case format to incoming string.
    /// </summary>
    /// <param name="source">String to apply formatting to.</param>
    /// <returns>Resulting string</returns>
    /// <remarks>
    /// Pascal renaming available through <cref="System.Globalization.TextInfo"/>
    /// </remarks>
    private string ApplyCase(string source)
    {
      if (source == null || source == String.Empty)
      {
        return source;
      }

      switch (CurrentCase)
      {
        case Cases.LOWER:
          return source.ToLower();
        case Cases.UPPER:
          return source.ToUpper();
        case Cases.PASCAL:
          TextInfo TextHelper = new CultureInfo("en-US", false).TextInfo;
          return TextHelper.ToTitleCase(source);
        default:
          return source;
      }
    }

    #endregion

  }
}
