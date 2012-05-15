/**
 * File: Id3.cs
 * Author: James McClure
 * 
 * Implements abstract Tag class for Id3v1 and Id3v2 
 * tags.
 * 
 * Revision History:
 * 0.2.2 2011-06-17
 * Updated copy ctor to include
 * musicbrainz/amazon data
 * 
 * 0.2.1 2011-06-14
 * Added copy constructor 
 * to fix pass by reference issue when creating
 * multiple tags
 * 
 * 0.2 2011-05-30 
 * Cleaned up comments
 * 
 * 0.1 2011-05-27 
 * Initial creation
 **/
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Common
{

  /// <summary>
  /// Implements id3v1 and v2 tags.
  /// </summary>
  public class Id3 : Metadata
  {

    #region Constructor

    /// <summary>
    /// Default constuctor. 
    /// </summary>
    /// <remarks>
    /// <see cref="mp3tag.Metadata.Art"/>
    /// Is the only non-base class attribute in Tag
    /// and must be initialized prior to being written to.
    /// </remarks>
    public Id3() { }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="id3">Tag to copy to new object</param>
    public Id3(Metadata id3)
    {
      this.Artist = id3.Artist;
      this.AlbumArtists = id3.AlbumArtists;
      this.Release = id3.Album;
      this.Title = id3.Title;
      this.Track = id3.Track;
      this.ReleaseYear = id3.ReleaseYear;
      this.Art = id3.Art;
      this.Genre = id3.Genre;
      this.MusicBrainzArtistId = id3.MusicBrainzArtistId;
      this.MusicBrainzReleaseId = id3.MusicBrainzReleaseId;
      this.MusicBrainzTrackId = id3.MusicBrainzTrackId;
      this.MusicBrainzReleaseType = id3.MusicBrainzReleaseType; 
      this.AmazonId = id3.AmazonId;
    }

    #endregion
    
    #region Tag
    
    /// <summary>
    /// Not implemented for leaf case in composite pattern.
    /// </summary>
    public void Remove(Id3 tag)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Not implemented for leaf case in composite pattern.
    /// </summary>
    public void Add(Id3 tag)
    {
      throw new NotImplementedException();
    }

#endregion

  }
}