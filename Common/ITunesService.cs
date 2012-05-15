/**
 * File: ITunesService.cs
 * Author: James McClure
 * 
 * Provides search functionality for
 * ITunes web store.
 * 
 * Revision History:
 * 0.1 2011-06-28
 * Initial creation
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace Common
{
  /// <summary>
  /// Class to search the ITunes store
  /// for metadata. Currently only acts
  /// as an accessory to <see cref="MusicBrainzService"/>
  /// which can not find album genres.
  /// </summary>
  class ITunesService : WebService
  {

    #region Attributes

    /// <summary>
    /// Stores root service address.
    /// </summary>
    private const string url = "http://ax.phobos.apple.com.edgesuite.net/WebObjects/MZStoreServices.woa/wa/wsSearch?";

    #endregion

    #region Ctors
    
    /// <summary>
    /// Searches service for supplied metadata.
    /// </summary>
    /// <param name="album">Metadata to search service for</param>
    /// <param name="maxResults">Maximum number of results to be returned
    /// by the service search</param>
    public ITunesService(File album, int maxResults)
    {
      Trace.WriteLine("Initializing MusicBrainz session");

      // Initialize search tokens
      Tokens = new Dictionary<string, string>();
      Metadata tag = album.Metadata.First();

      // Strip url-sensitive chars
      Regex pattern = new Regex("[()_-]");
      Tokens.Add("artist", pattern.Replace(tag.Artist, " "));
      Tokens.Add("album", pattern.Replace(tag.Album, " "));
      Tokens.Add("title", pattern.Replace(tag.Title, " "));

      MaxResults = maxResults;

      Trace.WriteLine("Session started");
    }

    #endregion

    #region WebService

    /// <summary>
    /// Currently not implemented
    /// </summary>
    /// <returns></returns>
    public override List<Metadata> Search()
    {
      List<Metadata> result = new List<Metadata>();
      return result;
    }
    
    #endregion

    #region Static
    
    /// <summary>
    /// Searches service for genre linked to specified
    /// album. 
    /// </summary>
    /// <param name="tag">Metadata to perform search for</param>
    /// <returns>Genre associated with the target album</returns>
    static public string SearchArtistGenre(Metadata tag)
    {
      // Build searcg string
      string QueryString = url
                           + "entity=album"
                           + "&term=" + tag.Artist;

      // Download JSON data string
      using (var webClient = new System.Net.WebClient())
      {
        String RawData = webClient.DownloadString(QueryString);
        JObject Feed = JObject.Parse(RawData);

        var entries = Feed["results"];
        foreach (var entry in entries)
        {
          return entry["primaryGenreName"].ToString();
        }
      }

      // Return empty string if nothing found
      return String.Empty;
    }

    #endregion

  }
}
