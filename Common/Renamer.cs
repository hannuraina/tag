/**
 * File: Renamer.cs
 * Author: James McClure
 *
 * Class is responsible for parsing
 * user formatted renaming functions.
 * The user can specify the order and format
 * of different file.Metadata to apply to the specified
 * string.
 * 
 * <example>
 * Renamer Expression = "%Artist%;%Album%;yo";
 * MyStringToFormat.Rename(Expression);
 * blink-182;dude ranch;yo
 * </example>
 * 
 * Revision History:
 * 0.2 2011-06-16 
 * Added use of string class extension
 * replace to make case insensitive matches 
 * on expression function
 * 
 * 0.1 2011-06-14
 * Initial Creation
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Common
{

  /// <summary>
  /// Allows user to specify formatting
  /// options for string renaming
  /// </summary>
  public class Renamer
  {

    #region Properties
    
    /// <summary>
    /// Gets and sets renaming function
    /// for folder based objects
    /// </summary>
    public string ReleaseFunction { get; set; }

    /// <summary>
    /// Gets and sets renaming function 
    /// for audio files
    /// </summary>
    public string SongFunction { get; set; }

    /// <summary>
    /// Gets and sets renaming function for 
    /// non-audio files
    /// </summary>
    public string FlatFunction { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// Apply renaming function to the specified
    /// string using data from input tag.
    /// </summary>
    /// <param name="music">Object to apply function to</param>
    /// <returns>Renamed string</returns>
    /// <remarks>
    /// Method needs access to the current objects
    /// file.Metadata. Therefore the client must provide
    /// a <see cref="mp3tag.Metadata"/> object.
    /// </remarks>
    public string Rename(File file)
    {
      string result = String.Empty;
      switch (file.FileType)
      {
        case FileTypes.RELEASE :
          result = ReleaseFunction;
          break;
        case FileTypes.SONG :
          result = SongFunction; break;
        case FileTypes.FLAT :
          result = FlatFunction; break;
      }

      Trace.WriteLine("Applying rename function: " + result + " to " + file.Metadata.Title);
      
      result = result.Replace("%Track%", file.Metadata.Track, StringComparison.OrdinalIgnoreCase);
      result = result.Replace("%ReleaseArtist%", file.Metadata.AlbumArtist, StringComparison.OrdinalIgnoreCase);
      result = result.Replace("%Artist%", file.Metadata.Artist, StringComparison.OrdinalIgnoreCase);
      result = result.Replace("%Release%", file.Metadata.Release, StringComparison.OrdinalIgnoreCase);
      result = result.Replace("%Title%", file.Metadata.Title, StringComparison.OrdinalIgnoreCase);
      result = result.Replace("%Genre%", file.Metadata.Genre, StringComparison.OrdinalIgnoreCase);
      result = result.Replace("%ReleaseYear%", file.Metadata.ReleaseYear, StringComparison.OrdinalIgnoreCase);

      return result;
    }

    #endregion

  }
}
