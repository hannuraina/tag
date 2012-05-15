/**
 * File: MusicBrainzService.cs
 * Author: James McClure
 * 
 * Web service wrapped for music brainz
 * 
 * Revision History:
 * 0.1 2011-06-14
 * File creation
 **/
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Timers;
using MusicBrainz;

namespace Common
{
  
  /// <summary>
  /// Wrapper class for musicbrainz web service.
  /// </summary>
  class MusicBrainzService : WebService
  {
    
    #region Constructors

    /// <summary>
    /// Constructor for musicbrainz service. Token set
    /// is built for search query based on incoming tag.
    /// </summary>
    /// <param name="tag">Metadata to search for</param>
    /// <param name="maxMaxResults">Number of MaxResults to return per search</param>
    public MusicBrainzService(Metadata meta, int maxResults=0)
    {
      Trace.WriteLine("Initializing MusicBrainz session");

      // Initialize search tokens
      Tokens = new Dictionary<string, string>();
      
      // Strip url-sensitive chars
      Regex pattern = new Regex("[()_-]|[0-9]{4}");
      string artist = pattern.Replace(meta.Artist ?? "", "?");
      string release = pattern.Replace(meta.Release ?? "", " ");
      string title = pattern.Replace(meta.Title ?? "", " ");

      // Build search list
      Tokens.Add("artist", artist == String.Empty ? "." : artist + "~");
      Tokens.Add("release", release == String.Empty ? "." : release + "~");
      Tokens.Add("title", title == String.Empty ? "." : title + "~");

      MaxResults = maxResults;
      Metadata = meta;

      Trace.WriteLine("Session started");
    }

    #endregion

    #region WebService
    
    /// <summary>
    /// Search service for specified token metadata
    /// </summary>
    /// <returns>List of mp3tag.Tags</returns>
    /// <remarks>
    /// MaxResults are added to the result set 
    /// from each type of search until the max return
    /// limit is hit. Searches are ordered based 
    /// on scope.
    /// 
    /// /// If the release is cached the search is ommitted.
    /// </remarks>
    public override List<Metadata> Search()
    {
      List<Metadata> results = new List<Metadata>();

      results.AddRange(TrackLookup(
            "track:\"" + Tokens["title"] + "\" "
          + "AND artist:" + Tokens["artist"] + " "
          + "AND type:(album OR ep)"));

      results.AddRange(AlbumLookup(
          "release:" + Tokens["release"] + "  "
        + "AND artist:" + Tokens["artist"] + " "));

      results.AddRange(AlbumLookup(
          "artist:" + Tokens["artist"] + " "));

      // Set source of the current data to MusicBrainz
      ((MetadataCollection)results).Source = MetadataSource.MUSICBRAINZ;
      return results;
    }
      
    #endregion

    #region Private Methods

    /// <summary>
    /// Searched service for data matching specified track title
    /// by artist
    /// </summary>
    /// <returns>List of tag collections</returns>
    /// <remarks>
    /// LINQ query is used to first select the tracks that match
    /// then the releases (Albums) associated with these
    /// tracks are returned. Appended letter 'a' to searches. MusicBrainz
    /// does not hit single words with tilde wildcards.
    /// </remarks>
    private List<Metadata> TrackLookup(string luceneQuery)
    {
      // Start timer for search
      DateTime timer = DateTime.Now;

      Trace.WriteLine("Lucene Track search " + luceneQuery);
      Query<Track> query = Track.QueryLucene(luceneQuery, MaxResults);

      List<Metadata> Results = (from a in query
                                from b in a.GetReleases()
                                select b.ToTag())
                                .ToList<Metadata>();

      string QueryTime = Math.Round((DateTime.Now - timer).TotalSeconds, 3).ToString() + "s";
      Trace.WriteLine("Search took " + QueryTime + ". MaxResults: " + Results.Count());
      return Results;
    }

    /// <summary>
    /// Searches service for album title and artist
    /// </summary>
    /// <returns>List of tag collections</returns>
    /// <remarks>
    /// LINQ query returns any albums that matched the 
    /// metadata criteria
    /// </remarks>
    private List<Metadata> AlbumLookup(string luceneQuery)
    {
      DateTime timer = DateTime.Now;

      Trace.WriteLine("Lucene Release search " + luceneQuery);
      Query<MusicBrainz.Release> query = MusicBrainz.Release.QueryLucene(luceneQuery, MaxResults);

      List<Metadata> Results = (from a in query
                                select a.ToTag())
                                .ToList<Metadata>();

      string QueryTime = Math.Round((DateTime.Now - timer).TotalSeconds, 3).ToString() + "s";
      Trace.WriteLine("Search took " + QueryTime + ". MaxResults: " + Results.Count());
      return Results;
    }

    #endregion

  }
}
