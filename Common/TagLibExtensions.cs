/**
 * File: TagLibExtensions.cs
 * Author: James McClure
 * 
 * Provides some extension methods for the tagging
 * library TagLib.
 * 
 * Revision History:
 * 0.2.4 2011-06-24
 * Added method for adding image metadata
 * 
 * 0.2.3 2011-06-10
 * Altered tag method to save file once tags are written.
 * 
 * 0.2.2 2011-06-09
 * Cleaned picture handling. Added single accessor (tag.Art)
 * 
 * 0.2.1 2011-06-02
 * Added Track# handling to get and set Tag accessors
 * 
 * 0.2 2011-05-31
 * Cleaned up comments
 * 
 * 0.1 2011-05-27 
 * Initial creation
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TagLib;

namespace Common
{
  
  /// <summary>
  /// Static class required by extension methods. 
  /// </summary>
  /// <remarks>
  /// Any frame information added to <see cref="mop3tag.Tag"/>
  /// must be added here for the change to be applied
  /// to the file. This is where the file is actually 
  /// being written to.
  /// </remarks>
  static public class TagLibExtensions
  {

    #region Static Methods

    /// <summary>
    /// Adds Get() method to <see cref="TagLib.Tag"/> which
    /// is read only by default.
    /// </summary>
    /// <param name="tag">Target tag</param>
    /// <returns>Casted tag</returns>
    /// <remarks>
    /// The tag is casted to <see cref="mp3tag.Metadata"/> prior to return.
    /// Used to initialize tag once <see cref="TagLib.File"/> is created
    /// in <see cref="mp3Tag.Music"/>.
    /// If not initialized, any updates to the tag fail with
    /// NullExceptions.
    /// </remarks>
    static public Metadata Get(this TagLib.Tag tag)
    {
      Metadata Id3 = new Id3();
      Id3.Artist = tag.FirstPerformer;
      Id3.Release = tag.Album;
      Id3.Title = tag.Title;
      Id3.ReleaseYear = tag.Year.ToString();
      Id3.Genre = tag.FirstGenre;
      Id3.Track = tag.Track.ToString();
      Id3.MusicBrainzReleaseType = tag.MusicBrainzReleaseType;
      Id3.MusicBrainzReleaseArtistId = tag.MusicBrainzArtistId;
      Id3.MusicBrainzArtistId = tag.MusicBrainzArtistId;
      Id3.MusicBrainzReleaseId = tag.MusicBrainzReleaseId;
      Id3.MusicBrainzTrackId = tag.MusicBrainzTrackId;
      Id3.MusicBrainzDiscId = tag.MusicBrainzDiscId; 
      return Id3;
    }

    /// <summary>
    /// Adds Set() method to <see cref="TagLib.Tag"/> to allow
    /// input of <see cref="Tag"/>.
    /// </summary>
    /// <param name="tag">Target tag</param>
    /// <param name="itag">Input tag</param>
    /// <returns>Updated Taglib.Tag object</returns>
    /// <remarks>
    /// <see cref="Tag"/> and <see cref="TagLib.Tag"/> implementation
    /// differs this method handles the translation. Added because
    /// value objects can't be passed by reference.
    /// 
    /// Pictures are handled in seperate extension
    /// </remarks>
    static public void Tag(this TagLib.File file, Metadata metadata)
    {
      file.Tag.Performers = new string[] { metadata.Artist };
      file.Tag.AlbumArtists = metadata.AlbumArtists;
      file.Tag.Album = metadata.Release;
      file.Tag.Title = metadata.Title;
      file.Tag.Year = uint.Parse(metadata.ReleaseYear);
      file.Tag.Genres = new string[] { metadata.Genre };
      file.Tag.Track = uint.Parse(metadata.Track);
      file.Tag.Comment = metadata.Comment;
      file.Tag.AmazonId = metadata.AmazonId;
      file.Tag.MusicBrainzArtistId = metadata.MusicBrainzArtistId;
      file.Tag.MusicBrainzReleaseType = metadata.MusicBrainzReleaseType;
      file.Tag.MusicBrainzReleaseArtistId = metadata.MusicBrainzReleaseArtistId;
      file.Tag.MusicBrainzReleaseId = metadata.MusicBrainzReleaseId;
      file.Tag.MusicBrainzTrackId = metadata.MusicBrainzTrackId;
      file.Tag.MusicBrainzDiscId = metadata.MusicBrainzDiscId;
      file.Save();
    }

    /// <summary>
    /// Embeds binary image data into metadata tag.
    /// </summary>
    /// <param name="file">TagLib.File to embed image data in</param>
    /// <param name="art">Image object</param>
    static public void SetImage(this TagLib.File file, Art art)
    {
      if (art.Url == String.Empty)
      {
        Trace.WriteLine("No image set for " + file.Name + "!");
        return;
      }

      
      file.Tag.Pictures = art.Picture;
      file.Save();
    }

    #endregion

  }
}
