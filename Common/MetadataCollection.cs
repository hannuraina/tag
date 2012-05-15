/**
 * File: TagCollection.cs
 * Author: James McClure
 * 
 * Provides collection implementation for 
 * Tag class. Primarily used when tagging full albums.
 * 
 * Revision History:
 * 0.2.5 2011-06-17
 * Added override properties
 * for amazon and musicbrainz items
 * 
 * 0.2.4 2011-06-16
 * Added method to grab first
 * tag in collection
 * 
 * 0.2.3 2011-06-14
 * Added Get() method for child tag indexing
 * 
 * 0.2.2 2011-06-09
 * Added trackCount property
 * 
 * 0.2.1 2011-06-02
 * Added image property
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
using System.Linq;
using TagLib;

namespace Common
{

  /// <summary>
  /// Provides collection implementation for <see cref="Metadata"/>
  /// </summary>
  public class MetadataCollection : Metadata, IEnumerable<Metadata>
  {

    #region Attributes

    /// <summary>
    /// Stores list of <see cref="Metadata"/> objects used
    /// for enumeration.
    /// </summary>
    private List<Metadata> collection;

    #endregion

    #region Properties

    /// <summary>
    /// Gets and sets source of the current metadata object.
    /// </summary>
    public override MetadataSource Source
    {
      get
      {
        return collection.Count > 0
               ? collection.First().Source
               : MetadataSource.NOTSPECIFIED;
      }
      set { foreach (Metadata tag in collection) tag.Source = value; }
    }

    /// <summary>
    /// Gets first childs music brainz track id. Does
    /// not have a set implementation.
    /// </summary>
    public override string MusicBrainzTrackId
    {
      get
      {
        return collection.Count > 0
           ? collection.First().MusicBrainzTrackId
           : null;
      }
    }

    /// <summary>
    /// Gets first childs music brainz release type
    /// </summary>
    public override string MusicBrainzReleaseType
    {
      get
      {
        return collection.Count > 0
               ? collection.First().MusicBrainzReleaseType
               : null;
      }
      set { foreach (Metadata tag in collection) tag.MusicBrainzReleaseType = value; }
    }

    /// <summary>
    /// Gets artist from first tag in 
    /// collection. <see cref="Metadata.Artist"/>. Setting
    /// writes artist across the collection.
    /// </summary>
    public override string Artist
    {
      get
      {
        return collection.Count > 0
               ? collection.First().Artist
               : null;
      }
      set { foreach (Metadata tag in collection) tag.Artist = value; }
    }

    /// <summary>
    /// Gets artist from first tag in 
    /// collection. <see cref="Metadata.Artist"/>. Setting
    /// writes artist across the collection.
    /// </summary>
    public override string AlbumArtist
    {
      get
      {
        return collection.Count > 0
               ? collection.First().FirstAlbumArtist
               : null;
      }
      set { foreach (Metadata tag in collection) tag.AlbumArtist = value; }
    }

    /// <summary>
    /// Gets album title from first tag in collection.
    /// Setting writes data across the collection.
    /// <see cref="Metadata.Album"/>
    /// </summary>
    public override string Release
    {
      get
      {
        return collection.Count > 0
               ? collection.First().Release
               : null;
      }
      set { foreach (Metadata tag in collection) tag.Release = value; }
    }

    /// <summary>
    /// Gets song title from first tag in collection.
    /// Setting writes data across the collection.
    /// <see cref="Metadata.Title"/>
    /// </summary>
    public override string Title
    {
      get
      {
        return collection.Count > 0
          ? collection.First().Title
          : null;
      }
    }

    /// <summary>
    /// Gets genre from first tag in collection.
    /// Setting writes data across the collection.
    /// <see cref="Metadata.Genre"/>
    /// </summary>
    public override string Genre
    {
      get
      {
        return collection.Count > 0
              ? collection.First().Genre
              : null;
      }
      set { foreach (Metadata tag in collection) tag.Genre = value; }
    }

    /// <summary>
    /// Gets release year from first tag in collection.
    /// Setting writes data across the collection.
    /// <see cref="Metadata.Year"/>
    /// </summary>
    public override string ReleaseYear
    {
      get
      {
        return collection.Count > 0
              ? collection.First().ReleaseYear
              : null;
      }
      set { foreach (Metadata tag in collection) tag.ReleaseYear = value; }
    }

    /// <summary>
    /// Gets and sets image url for all children.
    /// </summary>
    public override string Art
    {
      get 
      {
        return collection.Count > 0
              ? collection.First().Art
              : null;
      }
      set { foreach (Metadata tag in collection) tag.Art = value; }
    }

    /// <summary>
    /// Gets and sets amazon id 
    /// from all children.
    /// </summary>
    public override string AmazonId
    {
      get
      {
        return collection.Count > 0
              ? collection.First().AmazonId
              : null;
      }
      set { foreach (Metadata tag in collection) tag.AmazonId = value; }
    }

    /// <summary>
    /// Gets and sets music brainz artist id 
    /// from all children.
    /// </summary>
    public override string MusicBrainzArtistId
    {
      get 
      {
        return collection.Count > 0
              ? collection.First().MusicBrainzArtistId
              : null;
      }
      set { foreach (Metadata tag in collection) tag.MusicBrainzArtistId = value; }
    }

    /// <summary>
    /// Gets and sets music brainz release/album id 
    /// from all children.
    /// </summary>
    public override string MusicBrainzReleaseId
    {
      get
      {
        return collection.Count > 0
              ? collection.First().MusicBrainzReleaseId
              : null;
      }
      set { foreach (Metadata tag in collection) tag.MusicBrainzReleaseId = value; }
    }

    /// <summary>
    /// Gets title of first track
    /// </summary>
    public override string Track
    {
      get { return null; }
    }

    /// <summary>
    /// Gets number of collection contained in collection
    /// </summary>
    public override int TrackCount
    {
      get { return collection.Count; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Default constructor for the tag collection.
    /// Instantiates collection list.
    /// </summary>
    public MetadataCollection()
    {
      collection = new List<Metadata>();
    }

    /// <summary>
    /// Constructs collection from generic list of <see cref="Metadata"/>
    /// objects.
    /// </summary>
    /// <param name="tagList">Collection of collection</param>
    public MetadataCollection(List<Metadata> tagList)
    {
      collection = tagList;
    }

    /// <summary>
    /// Allows implicit casting of generic list of <see cref="Metadata"/> 
    /// objects to collection.
    /// </summary>
    /// <param name="tagList">List of collection to reference</param>
    /// <returns>Constructed tag collection object</returns>
    static public implicit operator MetadataCollection(List<Metadata> tagList)
    {
      return new MetadataCollection(tagList);
    }

    #endregion

    #region Metadata

    /// <summary>
    /// Formats collection in collection with the specified formatter.
    /// </summary>
    /// <param name="format">IFormatter used to format collection</param>
    public override void Format(Formatter format)
    {
      foreach (Metadata tag in collection)
      {
        tag.Format(format);
      }
    }

    /// <summary>
    /// Adds <see cref="Metadata"/> to list collection.
    /// </summary>
    /// <param name="tag">Tag to be added to collection</param>
    public override void Add(Metadata metadata)
    {
      collection.Add(metadata);
    }
    
    /// <summary>
    /// Removes <see cref="Metadata"/> from collection.
    /// </summary>
    /// <param name="tag">Tag to be removed</param>
    public override void Remove(Metadata tag)
    {
      collection.Remove(tag);
    }

    /// <summary>
    /// Prints out pretty version of tag
    /// </summary>
    public override string Print()
    {
      // ToString(G) outputs key value of the enum
      string output = String.Empty;
      output += "[Source]: " + Source.ToString("G") + "\r\n";
      output += "[Artist]: " + Artist + "\r\n";
      output += "[Album]: " + Release + "\r\n";
      output += "[Type]: " + MusicBrainzReleaseType + "\r\n";
      output += "[ReleaseYear]: " + ReleaseYear + "\r\n";
      output += "[Genre]: " + Genre + "\r\n";
      output += "[HasImage]: " + (Art != String.Empty).ToString()+ "\r\n";
      output += "[TrackCount]: " + TrackCount + "\r\n";
      output += "[ASIN]: " + AmazonId + "\r\n";
      output += "[Tracks]:";

      foreach (Metadata tag in collection)
      {
        output += "\r\n[" + tag.Track + "]" + tag.Title;
      }

      return output;
    }

    /// <summary>
    /// Returns tag at specified index
    /// </summary>
    /// <param name="index">Index of collection array to return</param>
    /// <returns>
    /// Tag at specified index
    /// </returns>
    /// <remarks>
    /// Throws exception when index is outside valid range
    /// </remarks>
    public override Metadata Get(int index)
    {
      if (collection.Count > 0 || collection.Count() >= index)
      {
        return collection.ElementAt(index);
      }
      else throw new IndexOutOfRangeException();
    }

    /// <summary>
    /// Helper method to grab first tag in collection
    /// </summary>
    /// <returns>First tag in collection</returns>
    public override Metadata First()
    {
      return collection.First();
    }

    #endregion

    #region IEnumerable

    /// <summary>
    /// Provides enumeration over collection.
    /// </summary>
    /// <returns>Collection enumerator</returns>
    public override IEnumerator<Metadata> GetEnumerator()
    {
      return collection.GetEnumerator();
    }

    /// <summary>
    /// Non-generic enumeration over collection. 
    /// Calls generic GetEnumerator().
    /// </summary>
    /// <returns>Non-Generic enumerator</returns>
    /// <remarks>
    /// Required by IEnumerable interface
    /// </remarks>
    //IEnumerator IEnumerable.GetEnumerator()
    //{
    //  return GetEnumerator();
    //}

#endregion

  }
}
