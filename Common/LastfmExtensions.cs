/**
 * File: LastfmExtension.cs
 * Author: James McClure
 * 
 * Extension methods for Lastfm Service.
 * 
 * Revision History:
 * 0.1 2011-06-14
 * File creation
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lastfm.Services;

namespace Common
{

  /// <summary>
  /// Contains extension method for last
  /// fm service call objects.
  /// </summary>
  static public class LastfmExtensions
  {

    #region Static

    /// <summary>
    /// Explicitly casts <see cref="Lastfm.Services.Album"/>
    /// to <see cref="mp3tag.Metadata"/>
    /// </summary>
    /// <param name="album">Album tag to be casted</param>
    /// <returns>mp3tag.Tag object</returns>
    static public Metadata ToTag(this Lastfm.Services.Album album)
    {
      // Set album specific data
      Metadata Collection = new MetadataCollection();
      Metadata Metadata = new Id3();
      Metadata.Artist = album.Artist.Name;
      Metadata.Release = album.Name;
      Metadata.ReleaseYear = album.GetReleaseDate().Year.ToString();
      Metadata.Art = album.GetImageURL(AlbumImageSize.ExtraLarge);
      
      // Invoke copy constructor on tag object
      // otherwirse all tags in the collection will have changes
      // applied.
      foreach (Track t in album.GetTracks())
      {
        Metadata.Title = t.Title;
        Metadata.Track = (Collection.ToList<Metadata>().Count() + 1).ToString();
        Collection.Add(new Id3(Metadata));
      }

      // Return tag collection for album
      return Collection;
    }

    #endregion

  }
}
