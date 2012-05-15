/**
 * File: Art.cs
 * Author: James McClure
 * 
 * Wrapper class for TagLib.Picture.
 * 
 * Revision History:
 * 0.4 2011-06-24
 * Added ctor for url strings
 * 
 * 0.3 2011-06-17
 * Added cover art search
 * 
 * 0.2.1 2011-06-10
 * Removed save and clear methods
 * 
 * 0.2 2011-06-09
 * Cleaned up abstraction of TagLib.Picture,
 * No longer visible to the user
 * Added implicit constructors from Art->Pictures
 * and vice-versa
 * 
 * 0.1.1 2011-06-07
 * Made Picture attribute public
 * 
 * 0.1 2011-06-03
 * File creation
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using System.IO;
using System.Net;
using System.Diagnostics;
using TagLib;

namespace Common
{

  /// <summary>
  /// Wrapper class for TagLib.Picture.
  /// </summary>
  /// <remarks>
  /// TagLib.Pictures requires images to present on
  /// the physical disk when updating the tag. This class
  /// provides a way to save image urls and only
  /// download/write the image data once a tag
  /// is selected as valid.
  /// </remarks>
  public class Art : File
  {

    #region Attributes

    /// <summary>
    /// Object that will be stored in the tag
    /// </summary>
    private TagLib.IPicture picture;

    #endregion

    #region Properties

    /// <summary>
    /// Accessor for image url string.
    /// </summary>
    /// <remarks>
    /// Most clients will interact with this
    /// property to manipulate image information.
    /// </remarks>
    public string Url
    {
      get { return Metadata.Art ?? ""; }
      set { Metadata.Art = value; }
    }

    public IPicture[] Picture
    {
      get { return new IPicture[] { picture };  }
    }

    /// <summary>
    /// Gets and sets object representation
    /// of the image data downloaded from url.
    /// </summary>
    public Image Image { get; set; }
    
    #endregion

    #region Static

    /// <summary>
    /// Stores cover art sites and the expression
    /// used to build an image uri.
    /// </summary>
    static public Dictionary<string, string> coverArtSites = 
      new Dictionary<string, string>() {
        {"amazon.us", "http://ec1.images-amazon.com/images/P/%ASIN%.01.LZZZZZZZ.jpg"},
        {"amazon.co.uk", "http://ec1.images-amazon.com/images/P/%ASIN%.02.LZZZZZZZ.jpg"},
        {"amazon.co.jp", "http://ec1.images-amazon.com/images/P/%ASIN%.09.LZZZZZZZ.jpg"},
        {"cdbaby.com", "http://cdbaby.name/%ALBUMCHAR0%/%ALBUMCHAR1%/%ALBUM%_large.jpg"},
      };

    #endregion

    #region Constructors

    /// <summary>
    /// Default ctor
    /// </summary>
    public Art()
    {
      Metadata = new Id3();
    }

    /// <summary>
    /// Constructor for art object
    /// </summary>
    /// <param name="path">Location of the file</param>
    /// <param name="depth">Depth in file heirarchy</param>
    public Art(string url)
    {
      Path = Parent.Path + "\\image" + url.Substring(url.Length - 4, 4).ToLower();
      Depth = Parent.Depth + 1;
      Metadata = new Id3();
    }


    /// <summary>
    /// Allows implicit cast from AttachedPictureFrame
    /// to Art.
    /// </summary>
    /// <param name="picture">Image data embedded in tag</param>
    /// <returns>Art object created from tag picture embed</returns>
    /// <remarks>
    /// Used AttachedPictureFrame instead of IPicture because 
    /// implicit casts to and from interfaces is not allowed. <see cref="TagLib.Picture"/>
    /// description holds path. This path is used for both image path
    /// and url. Checks for null picture, which occurs when no album art
    /// has been set.
    /// </remarks>
    static public implicit operator Art(TagLib.Id3v2.AttachedPictureFrame picture)
    {
      if (picture == null)
      {
        return new Art();
      }

      // Setup placeholder tag for renaming
      // & formatting.
      Art Art = new Art(picture.Description);
      Art.picture = picture;
      Art.Url = picture.Description;
      return Art;
    }

    /// <summary>
    /// Allows implicit/explicit cast to <see cref="TagLib.IPicture[]"/>
    /// from Art object.
    /// </summary>
    /// <param name="art">Object containing art data</param>
    /// <returns>Embeddable picture data object</returns>
    /// <remarks>
    /// Returns empty picture object if no art url/data exists.
    /// </remarks>
    static public implicit operator TagLib.IPicture[](Art art)
    {
      // Check for null art path, if none found return empty
      // picture object.
      return new IPicture[] { art.picture ?? new Picture() } ;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Searches various album art sites for 
    /// valid image urls. Based on the site uri
    /// definition relevant album info is appended.
    /// </summary>
    /// <returns>
    /// Image URL; Empty string if no valid image
    /// was found
    /// </returns>
    /// <remarks>
    /// Valid urls are tested by attempting to connect to 
    /// the address. If found, the uri is returned. 
    /// </remarks>
    public string Search(Metadata Metadata)
    {
      string uri = String.Empty;

      foreach (KeyValuePair<string, string> site in coverArtSites)
      {
        // Skip amazon searches if no ASIN is set
        if (site.Key.Contains("amazon") && Metadata.AmazonId == null)
        {
          continue;
        }

        uri = site.Value;
        uri = uri.Replace("%ASIN%", Metadata.AmazonId);
        uri = uri.Replace("%ARTIST%", Metadata.FirstAlbumArtist.ToLower());
        uri = uri.Replace("%ALBUM%", Metadata.Release.ToLower());
        uri = uri.Replace("%MBRELEASEID%", Metadata.MusicBrainzReleaseId);
        uri = uri.Replace("%MBARTISTID%", Metadata.MusicBrainzArtistId);
        uri = uri.Replace("%ALBUMCHAR0%", Metadata.Release.Substring(0, 1).ToLower());
        uri = uri.Replace("%ALBUMCHAR1%", Metadata.Release.Substring(1, 1).ToLower());

        Trace.WriteLine("Testing " + uri);

        if (ValidUri(uri))
        {
          Trace.WriteLine("Image found!");
          Url = uri;
          return Url;
        }
      }

      return String.Empty;
    }

    /// <summary>
    /// Downloads image data from url stored in 
    /// <see cref="url"/>. Image saved with name 
    /// specified by incoming filename parameter.
    /// </summary>
    /// <param name="filename">Name of saved image</param>
    public void Download()
    {
      // No image art specified
      // Remove reference from parent object
      if (Url == String.Empty)
      {
        Delete();
        return;
      }

      Trace.WriteLine("Downloading image from " + Url);
      Path = Parent.Path + "\\image" + Metadata.Art.Substring(Url.Length - 4, 4);

      try
      {
        // Set client options. Specify agent so client isn't rejected
        HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create(Url);
        Request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
        Request.Referer = "http://www.google.com/";
        Request.AllowWriteStreamBuffering = true;
        Request.Timeout = 20000;

        // Convert webstream to image
        using (WebResponse Response = Request.GetResponse())
        {
          Stream Stream = Response.GetResponseStream();
          Image = Image.FromStream(Stream);
        }

        Save();
      }
      catch (Exception e)
      {
        Trace.WriteLine("Could not download image from " + Url + ". " + e.Message);
      }
    }

    /// <summary>
    /// Writes image data to disk.
    /// </summary>
    /// <remarks>
    /// The image must be stored on disk before the tag
    /// data can be written. Image type always defaults
    /// to <see cref="TagLib.PictureType.FrontCover"/>
    /// </remarks>
    public void Save()
    {
      Trace.WriteLine("Saving " + Name + Extension);

      if(Exists())
      {
        // Write image data to disk
        Image.Save(Path);

        // Create tag-compliant Picture object
        picture = new Picture(Path);
        picture.Type = PictureType.FrontCover;
      }
    }

    #endregion

    #region Private Methods
  
    /// <summary>
    /// Checks for valid image uri
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    private bool ValidUri(string uri)
    {
      HttpWebRequest Request = (HttpWebRequest)HttpWebRequest.Create(uri);
      Request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1)";
      Request.Referer = "http://www.google.com/";
      Request.AllowWriteStreamBuffering = true;
      Request.Timeout = 1000;

      try
      {
        using (HttpWebResponse Response = (HttpWebResponse)Request.GetResponse())
        {
          return Response.StatusCode == HttpStatusCode.OK;
        }
      }
      catch (Exception)
      {
        return false;
      }
    }

    #endregion

    #region Music

    /// <summary>
    /// File with <see cref="name"/> currently
    /// exists on disk.
    /// </summary>
    /// <returns>True if object exists, false otherwise</returns>
    public override bool Exists()
    {
      //return System.IO.File.Exists(Path) && image != null;
      return Url != String.Empty && Url != null;
    }

    /// <summary>
    /// Not implemented for this class.
    /// </summary>
    public override void Collapse()
    {
      throw new NotImplementedException();
    }

    #endregion

  }
}
