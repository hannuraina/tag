/**
 * File: Tag.cs
 * Author: James McClure
 * 
 * Abstract class for common audio metadata.
 * Extends TagLib.Id3v2.Tag to easily write to 
 * Taglib.File which provides underlying structure
 * for audio files in the application.
 * 
 * Revision History:
 * 0.2.5 2011-06-16
 * Added virtual method to grab first
 * tag in collection
 * 
 * 0.2.4 2011-06-15
 * Added checks for blank artist and 
 * album names so that initial metadata 
 * takes preference.
 * 
 * 0.2.3 2011-06-09
 * Added trackCount property
 * 
 * 0.2.2 2011-06-02 
 * Fixed TrackNumber property by adding
 * field to TagLibExtensions. 
 * Added default value for track number
 * Added image property/attribute
 * 
 * 0.2.1 2011-06-01
 * Added Number property
 * 
 * 0.2 2011-05-31 
 * Cleaned up comments
 * 
 * 0.1 2011-05-27 
 * Initial creation
 **/
using System;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using TagLib;

namespace Common
{

  #region Enum

  /// <summary>
  /// Source of metadata to be applied
  /// </summary>
  public enum MetadataSource
  {
    MUSICBRAINZ=0,
    LASTFM,
    NOTSPECIFIED
  }

  #endregion

  /// <summary>
  /// Metadata container for files.
  /// </summary>
  /// <remarks>
  /// Extends <see cref="TagLib.Id3v2.Tag"/>
  /// allowing easy tag manipulation of <see cref="File"/> and its base
  /// structure <see cref="TagLib.File"/>. Provides abstraction for
  /// composite design (component actor). Asbtracts enumeration 
  /// over a list of <see cref="Metadata"/> objects for loop usage (foreach, etc.)
  /// </remarks>
  public abstract class Metadata : TagLib.Id3v2.Tag, IEnumerable<Metadata>
  {

    #region Properties

    /// <summary>
    /// Gets and sets source of metadata
    /// for current object
    /// </summary>
    public virtual MetadataSource Source { get; set; }

    /// <summary>
    /// Stores url of album image.
    /// </summary>
    /// <remarks>
    /// The only non-base class attribute in Tag.
    /// Must be initialized prior to being written to.
    /// </remarks>
    public virtual String Art { get; set; }

    /// <summary>
    /// Gets and sets for FirstPerformer id3 frame.
    /// </summary>
    /// <remarks>
    /// Syntactic sugar for the user since Artist 
    /// is more common than 'performer'.
    /// </remarks>
    public virtual string Artist
    {
      get { return FirstPerformer; }
      set
      {
        if (value != String.Empty && value != null)
        {
          Performers = new string[] { value };
          
          // test notification bubbling to gui
          
        }
      }
    }

    /// <summary>
    /// Gets and sets for Genres id3 frame.
    /// </summary>
    /// <remarks>
    /// Genre defaults to an list structure in <see cref="TagLib.Id3v2.Tag"/>
    /// allows user to set genre as string.
    /// </remarks>
    public virtual string AlbumArtist
    {
      get { return FirstAlbumArtist ?? ""; }
      set { AlbumArtists = new string[] { value }; }
    }

    /// <summary>
    /// Gets and sets for Genres id3 frame.
    /// </summary>
    /// <remarks>
    /// Genre defaults to an list structure in <see cref="TagLib.Id3v2.Tag"/>
    /// allows user to set genre as string.
    /// </remarks>
    public virtual string Genre
    {
      get { return FirstGenre ?? ""; }
      set { Genres = new string[] { value }; }
    }

    /// <summary>
    /// Gets and sets for uint based year id3 frame.
    /// </summary>
    /// <remarks>
    /// Converts uint to string and vice-versa.
    /// </remarks>
    public virtual string ReleaseYear
    {
      get { return Year.ToString(); }
      set { Year = uint.Parse(value); }
    }

    /// <summary>
    /// Gets and sets track number for the current
    /// object , if one exists. Abstraction of
    /// <see cref="TagLib.Tag.Track"/>
    /// </summary>
    /// <remarks>
    /// Track stored as uint by default in metadata
    /// conversion is required and defaults to '00' 
    /// format.
    /// </remarks>
    public new virtual string Track
    {
      // Pad return with zeroes
      get { return String.Format("{0:00}", base.Track); }
      set
      {
        // Number default to zero
        if (value != String.Empty && value != null)
        {
          base.Track = uint.Parse(value);
        }
      }
    }

    /// <summary>
    /// Gets and sets album title with 
    /// special logic before-hand
    /// </summary>
    public virtual string Release
    {
      get { return Album; }
      set 
      {
        if (value == null) return;
        else
          Album = value.Trim();
      }
    }
    /// <summary>
    /// Gets number of tags contained in collection.
    /// Defaults to 1.
    /// </summary>
    public new virtual int TrackCount
    {
      get { return 1; }
    }

    #endregion

    #region Virtual

    /// <summary>
    /// Add tag to child collection if one exists
    /// </summary>
    public virtual void Add(Metadata tag)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Remvoes specified tag from child collection if one exists
    /// </summary>
    public virtual void Remove(Metadata tag)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Formats tag Fields with specifed formatter
    /// </summary>
    public virtual void Format(Formatter formatter)
    {
      Artist = formatter.Format(null, Artist);
      Album = formatter.Format(null, Album);
      Title = formatter.Format(null, Title);
      Genre = formatter.Format(null, Genre);
      MusicBrainzReleaseType = formatter.Format(null, MusicBrainzReleaseType);

    }

    /// <summary>
    /// Prints out pretty version of tag
    /// </summary>
    public virtual string Print()
    {
      string output = String.Empty;
      output += "[Source]: " + Source.ToString("G") + "\r\n";
      output += "[Artist]: " + Artist + "\r\n";
      output += "[Release]: " + Release + "\r\n";
      output += "[Title]: " + Title + "\r\n";
      output += "[ReleaseYear]: " + ReleaseYear + "\r\n";
      output += "[Genre]: " + Genre + "\r\n";
      output += "[Track]: " + Track + "\r\n";
      output += "[HasImage]: " + (Art == String.Empty).ToString() + "\r\n";
      output += "[TrackCount]: " + TrackCount + "\r\n";
      output += "[ASIN]: " + AmazonId;
      return output;
    }

    /// <summary>
    /// Returns child element at specified index. Current item
    /// if no child exist.
    /// </summary>
    /// <param name="index">Indexed child to return</param>
    /// <returns>Tag at specified index</returns>
    public virtual Metadata Get(int index)
    {
      return this;
    }

    /// <summary>
    /// Helper method to return first element
    /// of tag collection.
    /// </summary>
    /// <returns>First tag in collection</returns>
    /// <remarks>
    /// For single tag elements the object itself
    /// is returned.
    /// </remarks>
    public virtual Metadata First()
    {
      return this;
    }

    #endregion
  
    #region IEnumerable

    /// <summary>
    /// Provides generic enumeration over child <see cref="Metadata"/> collection.
    /// </summary>
    /// <returns>Collection enumerator</returns>
    /// <remarks>
    /// If no collection exists (ie leaf node) the object will
    /// return itself.
    /// </remarks>
    public virtual new IEnumerator<Metadata> GetEnumerator()
    {
      yield return this;
    }

    /// <summary>
    /// Provides collection enumeration
    /// </summary>
    /// <returns>Collection enumerator</returns>
    /// <remarks>
    /// Required by IEnumerable, simply calls generic
    /// method GetEnumerator()
    /// </remarks>
    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

#endregion

  }
}
