/**
 * File: LastfmService.cs
 * Author: James McClure
 * 
 * Wrapper class for last fm/audioscrobbler
 * web service.
 * 
 * Revision History:
 * 0.1.1 2011-06-13
 * Moved max result count into ctor
 * 
 * 0.1 2011-06-07
 * File creation
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Diagnostics;
using Lastfm.Services;

namespace Common
{

  #region Enum

  /// <summary>
  /// Stores values for page access
  /// instead of hard coding values.
  /// </summary>
  /// <remarks>
  /// For the time being only page one, at most,
  /// is being read for efficiency reasons.
  /// </remarks>
  public enum Pages
  {
    FIRST=1,
    SECOND,
    THIRD,
    FOURTH,
    FIFTH
  }

  #endregion

  /// <summary>
  /// Implementation of <see cref="WebService"/>
  /// for metadata search services provided by last.fm.
  /// </summary>
  class LastfmService : WebService
  {

    #region Attributes

    /// <summary>
    /// Stored API key for last.fm authentication
    /// </summary>
    private string key = "a5d060187f3a863f67770a9872bd9da0";
    
    /// <summary>
    /// Stores secret key for last.fm authentication
    /// </summary>
    private string secret = "8d5e541b887926019b684bd71719e3ed";
    
    /// <summary>
    /// Last.fm username. Used for authentication
    /// </summary>
    private string username = "hannuraina";

    /// <summary>
    /// Last.fm password.
    /// </summary>
    private string passwd = "100265";
    
    /// <summary>
    /// Stores last.fm authenticated session used
    /// for performing searches with the service.
    /// </summary>
    private Session session;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructs last.fm service object 
    /// assigning data from tag into local search
    /// tokens. A last.fm authenticated session
    /// is also setup based on user parameters and keys.
    /// </summary>
    /// <param name="tag">Song tag to search for</param>
    public LastfmService(File release, int maxMaxResults=20)
    {
      Trace.WriteLine("Initializing LastFM Session");
      Metadata metadata = release.Metadata;

      // Initialize search tokens
      Tokens = new Dictionary<string, string>();
      Tokens.Add("artist", metadata.Artist);
      Tokens.Add("release", metadata.Release);
      Tokens.Add("title", metadata.Title);

      MaxResults = maxMaxResults;

      // Initialize lastfm session
      session = new Session(key, secret);
      session.Authenticate(username, Lastfm.Utilities.md5(passwd));
      Trace.WriteLine("Session established with key: " + session.SessionKey);
    }

    #endregion

    #region WebService

    /// <summary>
    /// Implementation of abstract search method.
    /// Calls 3 types of searches based on stored
    /// search tokens.
    /// </summary>
    /// <param name="maxMaxResults">Number of MaxResults to be
    /// returned</param>
    /// <returns>Collection of album tags</returns>
    /// <remarks>
    /// Runs most specific search first (TrackLookup)
    /// which has the highest chance of being an exact match.
    /// Next, GenericLookup() is run which searches for full text
    /// string combining artist name and album name.
    /// Finally, artistsearch() which has the smallest chance
    /// of being a exact find. The result count is decremented
    /// after each search.
    /// 
    /// If the release is cached the search is ommitted.
    /// </remarks>
    public override List<Metadata> Search()
    {
      List<Metadata> results = new List<Metadata>();

      // Perform searches
      results.AddRange(TrackLookup());
      results.AddRange(AlbumLookup());
      results.AddRange(ArtistLookup());

      ((MetadataCollection)results).Source = MetadataSource.MUSICBRAINZ;
      return results;
    }

    #endregion

    #region Private Methods
    
    /// <summary>
    /// Perform search based on artist name and 
    /// one of the track titles from the album.
    /// </summary>
    /// <param name="maxMaxResults">Number of MaxResults to return</param>
    /// <returns>Collection of album tags</returns>
    private List<Metadata> TrackLookup()
    {
      Trace.WriteLine("Performing track lookup. Artist: "
                      + Tokens["artist"]
                      + " Track Title: "
                      + Tokens["title"]);

      
      TrackSearch search = new TrackSearch(Tokens["artist"], Tokens["title"], session);
      search.SpecifyItemsPerPage(MaxResults);
      List<Metadata> tmp = (from track in search.GetPage((int)Pages.FIRST)
                       select track.GetAlbum().ToTag()).ToList<Metadata>();
      MaxResults = MaxResults - tmp.Count();

      Trace.WriteLine("MaxResults: " + tmp.Count());
      return tmp;
    }

    /// <summary>
    /// Perform generic lookup based on artist name and album
    /// title.
    /// </summary>
    /// <param name="maxMaxResults">Number of MaxResults to return</param>
    /// <returns>Collection of album tags</returns>
    /// <remarks>
    /// Is actually calling albumSearch() which accepts both
    /// artist and album as input.
    /// </remarks>
    private List<Metadata> AlbumLookup()
    {
      Trace.WriteLine("Performing album  lookup. Artist: "
                      + Tokens["artist"]
                      + " Release: "
                      + Tokens["release"]);
      
      AlbumSearch search = new AlbumSearch(Tokens["artist"] + " - " + Tokens["release"], session);
      search.SpecifyItemsPerPage(MaxResults);
      List<Metadata> tmp = (from album in search.GetPage((int)Pages.FIRST)
                       select album.ToTag()).ToList<Metadata>();
      MaxResults = MaxResults - tmp.Count();

      Trace.WriteLine("MaxResults: " + tmp.Count());
      return tmp;
    }

    /// <summary>
    /// Perform artist based search.
    /// </summary>
    /// <param name="maxMaxResults">Number of MaxResults to return</param>
    /// <returns>Collection of album tags</returns>
    /// <remarks>
    /// Method first pulls matched artists and then 
    /// based on this result pulls associated albums. Most searches
    /// should not reach this lookup.
    /// </remarks>
    private List<Metadata> ArtistLookup()
    {
      Trace.WriteLine("Performing artist lookup. Artist: "
                      + Tokens["artist"]);
      
      ArtistSearch search = new ArtistSearch(Tokens["artist"], session);
      search.SpecifyItemsPerPage(MaxResults);
      List<Metadata> tmp = (from artist in search.GetPage((int)Pages.FIRST)
                       from album in artist.GetTopAlbums()
                       select album.Item.ToTag()).ToList<Metadata>();
      MaxResults = MaxResults - tmp.Count();

      Trace.WriteLine("MaxResults: " + tmp.Count());
      return tmp;
    }

    #endregion

  }
}
