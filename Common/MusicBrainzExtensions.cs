/**
 * File: MusicBrainzExtension.cs
 * Author: James McClure
 *
 * Adds functionality to MusicBrainz
 * metadata search library. Functionality
 * includes conversion to application Tag format,
 * additional metadata collection from 
 * web service: art, gere, release year, etc...
 * 
 * Revision History
 * 0.2.2 2011-06-28
 * Changed artist listing to track based
 * instead of pulling from the album for various artist
 * discs.
 * 
 * 0.2.1 2011-06-17
 * Added MusicBrainzId's to tag creation
 * 
 * 0.2 2011-06-16 
 * Added release date to query.
 * 
 * 0.1 2011-06-13
 * Initial Creation
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using MusicBrainz;

namespace Common
{
  /// <summary>
  /// Extends base music brainz library
  /// for application specific purposes.
  /// </summary>
  static public class MusicBrainzExtensions
  {

    #region Static 

    /// <summary>
    /// Converts musicbrainz metadata collection to 
    /// <see cref="mp3tag.MetadataCollection"/>.
    /// </summary>
    /// <param name="album">Musicbrainz album object</param>
    /// <returns>Corresponding tag collection</returns>
    /// <remarks>
    /// Musicbrainz supports release types. This information
    /// in the case of non-album releases the data is appended
    /// to the album name. Date defaults to today if nothing 
    /// is set.
    /// 
    /// In the case of various artist, the artist is set to
    /// album artist first to locate album art & genre. 
    /// Then artist is based on a per-track basis.
    /// </remarks>
    static public Metadata ToTag(this MusicBrainz.Release album)
    {
      Metadata Collection = new MetadataCollection();
      Metadata Metadata = new Id3();

      // If date is not set, use current date
      string ReleaseDate = String.Empty;
      try {
        ReleaseDate = album.GetEvents().First().Date.ToString();
      } 
      catch(Exception)
      {
        ReleaseDate = DateTime.Now.Year.ToString();
      }
      
      // Build tag with search data
      Art Art = new Art();
      Metadata.MusicBrainzReleaseArtistId = album.GetArtist().Id;
      Metadata.MusicBrainzReleaseId = album.Id;
      Metadata.MusicBrainzReleaseType = album.GetReleaseType().ToString();
      Metadata.AlbumArtist = album.GetArtist();
      Metadata.Release = album.GetTitle();
      Metadata.AmazonId = album.GetAsin();
      Metadata.ReleaseYear = ReleaseDate.Substring(0, 4);
      Metadata.Art = Art.Search(Metadata);

      // Search for genre; populate with default for now
      Metadata.Genre = "Hardcore";
      //Metadata.Genre = ITunesService.SearchArtistGenre(Metadata);

      // Set track titles
      foreach (Track Trk in album.GetTracks())
      {
        Metadata.Title = Trk.GetTitle();
        Metadata.Artist = Trk.GetArtist();
        Metadata.MusicBrainzArtistId = Trk.GetArtist().Id;
        Metadata.Track = (Collection.ToList<Metadata>().Count() + 1).ToString();
        Collection.Add(new Id3(Metadata));
      }

      return Collection;
    }

    #endregion

  }
}

