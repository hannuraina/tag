/**
 * File: Song.cs
 * Author: James McClure
 * 
 * Provides implementaton for the abstract class Music.
 * Represents a lead node or single file in the file tree.
 * 
 * Revision History:
 * 0.3.2 2011-07-18
 * Added logic for audio format conversion
 * 
 * 0.3.1 2011-06-23
 * Added checksum functionality
 * 
 * 0.3 2011-06-16
 * Added filename artist parsing.
 * 
 * 0.2.5 2011-06-14
 * Changed format method to format
 * the song's tag. Giving renaming attribute selection
 * choice to the user
 * Added most recent formatter attribute
 * to allow formatting prior to renaming
 * 
 * 0.2.4 2011-06-10
 * Updated save method to first clear then apply 
 * local tag before saving
 * 
 * 0.2.3 2011-06-03
 * Added filetype check for song creation
 * Added exists helper method
 * 
 * 0.2.2 2011-06-01
 * Added ParseTrackNumber method to extract 
 * track numbers from file name
 * 
 * 0.2.1 2011-05-31
 * Removed non-implemented methods to class abstraction
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
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Text;
using TagLib;

namespace Common
{
  
  #region Enum
  
  /// <summary>
  /// Represents hard coded levels of object depth in 
  /// the file tree.
  /// </summary>
  /// <example>
  /// Song.Move(this.Depth - Scope.ROOT)
  /// Would move the current file to the root level
  /// </example>
  /// <remarks>
  /// Primarily used when moving files up and down the 
  /// file tree.
  /// </remarks>
  public enum Scope
  {
    ROOT=0,
    LEVEL1,
    LEVEL2,
    LEVEL3,
    LEVEL4,
    LEVEL5
  };

  #endregion

  /// <summary>
  /// Implements <see cref="Music"/> class to represent 
  /// leaf nodes in the file tree. In this context the objects
  /// represent songs, files with associated metadata. 
  /// Commonly songs are children of <see cref="Release"/> objects.
  /// </summary>
  /// <seealso cref="Music"/>
  public class Song : File
  {

    #region Static

    /// <summary>
    /// Stores list of acceptable filetypes
    /// for Song object creation.
    /// </summary>
    static public List<string> SongTypes = new List<string>() { ".mp3", ".m4a", ".wma", ".ogg" };

    #endregion

    #region Constructors

    /// <summary>
    /// Instantiates a new Song object along with a pysical
    /// object represented by a <see cref="TagLib.File"/> object.
    /// </summary>
    /// <param name="path">Absolute path of the object</param>
    public Song(string path, int depth=0)
    {
      // Set location of the song file
      Path = path;
      Depth = depth;

      // File is not a supported type (*.jpg, *.nfo, etc)
      // Do not create audio file but do apply
      // placeholder tag to the file.
      if (!Song.SongTypes.Contains(Extension))
      {
        Delete();
        return;
      }

      // Convert file if format does not match
      // user specification
      if ("." + TargetFormat.ToString("G") != Extension.ToUpper())
      {
        Path = AudioConverter.Convert(this, TargetFormat);
      }

      // Setup base TagLib.File and associated Tag
      Base = TagLib.File.Create(Path);
      Metadata tmp = Base.Tag.Get();
      Metadata = tmp;

      // Parse extra metadata from filename
      ParseTrackFromFilename();
      ParseArtistFromFilename();

      // If title is not set use file name
      Metadata.Title = Metadata.Title ?? Name;

      // Check for valid metadata
      if (Metadata.Artist == null) Console.WriteLine("no artist set");
      if (Metadata.Release == null) Console.WriteLine("no album set");
      if (Metadata.Track == null) Console.WriteLine("no track set");
    }

    /// <summary>
    /// Provides implict casting from <see cref="string"/> to <see cref="Song"/>
    /// </summary>
    /// <param name="path">Absolute path of the file</param>
    /// <returns>New song object</returns>
    static public implicit operator Song(string path)
    {
      return new Song(path);
    }

    #endregion

    #region File

    /// <summary>
    /// Delete current node from parent child listing
    /// and from physical disk.
    /// </summary>
    /// <remarks>
    /// Class based operation are handled by the base class
    /// such as deletion from parent child listing.
    /// </remarks>
    public override void Delete()
    {
      Trace.WriteLine("Deleting file " + Path);
      base.Delete();
      System.IO.File.Delete(Path);
    }

    /// <summary>
    /// Move current node to highest level below root.
    /// </summary>
    /// <example>
    /// Music foo = root/bar/bar1/bar2/foo.mp3
    /// foo.Collapse();
    /// foo #root/bar/foo.mp3
    /// </example>
    /// <remarks>
    /// Only move files that are atleast two (2) levels
    /// nested from root. This prevents accidentally moving files at the 
    /// level root + 1 or even root itself!
    /// </remarks>
    /// <seealso cref="Music.Move"/>
    public override void Collapse()
    {
      if (Depth > (int)Scope.LEVEL2)
      {
        Move(Depth - (int)Scope.LEVEL2);
      }

      // Delete non-audio files
      if (!SongTypes.Contains(Extension))
      {
        Delete();
        return;
      } 
    }

    /// <summary>
    /// Checks whether file exists
    /// </summary>
    /// <returns>True if exists, false otherwise</returns>
    public override bool Exists()
    {
      return System.IO.File.Exists(Path);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Attempts to extract track number from filename.
    /// </summary>
    /// <remarks>
    /// Method checks for possible track number in filename.
    /// If one is found itthe current objects tag is updated
    /// else the existing tag. Will
    /// overwrite the initial metadata from the file if
    /// a match is found.
    /// </remarks>
    private void ParseTrackFromFilename()
    {
      Match matches = Regex.Match(Name, 
                      @"((?<!\w)(?<track>\d{1,2})(?!\w|$))", 
                      RegexOptions.IgnoreCase);
      string match = matches.Groups["track"].Value.Trim();
      Metadata.Track = match == String.Empty ? null : match;

      Trace.WriteLine("Parsed track [" + match + "] from " + Name + Extension);
    }

    /// <summary>
    /// Attempts to extract artist name from filename.
    /// </summary>
    /// <remarks>
    /// Matches anything proceeded by a hyphen. Will
    /// overwrite the initial metadata from the file if
    /// a match is found.
    /// </remarks>
    private void ParseArtistFromFilename()
    {
      Match matches = Regex.Match(Name,
                      @"(^|(?<=\d)\.|-|(?<=\d)\s)(?<artist>([\D\.]+?[\s\d]*?))-",
                      RegexOptions.IgnoreCase);
      string match = matches.Groups["artist"].Value.Trim();
      Metadata.Artist = Metadata.Artist == null
                        ? match == String.Empty 
                          ? null 
                          : match
                        : Metadata.Artist;

      Trace.WriteLine("Parsed artist [" + Metadata.Artist + "] from " + Name + Extension);
    }
  }

  #endregion

}
