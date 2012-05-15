/**
 * File: FileComparer.cs
 * Author: James McClure
 * 
 * Implements sorting methods for File objects
 * based on accessible metadata.
 * 
 * Revision History:
 * 0.1.1 2011-06-03
 * Added check for folders and non-audio
 * files in sort
 * 
 * 0.1 2011-06-02
 * Initial creation.
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{

  #region Enum
  
  /// <summary>
  /// Specifies the type of sorting to be 
  /// performed.Default is track number.
  /// </summary>
  public enum MusicCompareTypes
  {
    TRACKNUMBER=0,
    TITLE=1
  }

  #endregion

  /// <summary>
  /// Sorts Music-based collections based
  /// on user criteria.
  /// </summary>
  public class FileComparer : IComparer<File>
  {

    #region Properties

    public MusicCompareTypes Type { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Default ctor. Sets the type of sort to 
    /// perform.
    /// </summary>
    /// <param name="compare"></param>
    public FileComparer(MusicCompareTypes compare)
    {
      Type = compare;
    }

#endregion

    #region IComparer

    /// <summary>
    /// Override base compare function to perform sort
    /// operations on Music objects.
    /// </summary>
    /// <param name="obj1">Comparer</param>
    /// <param name="obj2">Comparee</param>
    /// <returns>True is comparer is smaller, false otherwise</returns>
    /// <remarks>
    /// Uses base type compareto method to determine which value
    /// is larger. Defaults to smaller. Directories/non-aduio files 
    /// will be inserted at beginning of the collection.
    /// </remarks>
    public int Compare(File file1, File file2)
    {
      // New file is folder or non-audio default to beginning of list
      if (!Song.SongTypes.Contains(file2.Extension))
      {
        return 0;
      }

      // Testing object is folder or non-audio
      // push new object to next position
      if (!Song.SongTypes.Contains(file1.Extension))
      {
        return 1;
      }

      switch (Type)
      {
        case MusicCompareTypes.TRACKNUMBER:
          return file1.Metadata.Track.CompareTo(file2.Metadata.Track);
        case MusicCompareTypes.TITLE:
          return file1.Metadata.Title.CompareTo(file2.Metadata.Title);
        default:
          return 0;
      }
    }

    #endregion

  }
}
