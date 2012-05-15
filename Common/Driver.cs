/**
 * File: Program.cs
 * Author: James McClure
 * 
 * Mp3Tag is an application used to lessen the manual 
 * portion of renaming mp3 files. Using various
 * webservices (Audioscrobbler, musicbrainz and amazon)
 * existing albums are tagged and formatted according
 * to user preferences.
 * 
 * Notes:
 * TagCollections, lists of Tag objects, are representative
 * of album releases in this application.
 * 
 * When adding id3 fields, the copy constructor in the id3 
 * class must be updated to include said fields.
 * 
 * Revision History:
 * 0.3.0 2011-07-29
 * Fixed bugs with cache store and 
 * iterating over releases
 * 
 * 0.2.9 2011-07-20
 * Optimized Musicbrainz search
 * 
 * 0.2.8 2011-07-19
 * Refactored code to display search results, user
 * input.
 * Added .wma -> .mp3 conversion
 * 
 * 0.2.7 2011-07-15
 * Added SQLite cache support.
 * 
 * 0.2.6 2011-06-28
 * Added support for various artists albums
 * 
 * 0.2.5 2011-06-27
 * Added hash file and checksum to comments
 * section of metadata.
 * 
 * 0.2.4 2011-06-23
 * Fixed issue with collapsing folder heirarchy
 * Adjusted art search to return once match found
 * 
 * 0.2.3 2011-06-22
 * Updated MusicBrainz Query library
 * to set result limit
 * 
 * 0.2.2 2011-06-16
 * Added logging
 * 
 * 0.2.1 2011-06-15
 * Song will now attempt to extact
 * artist name and track number from the filename. 
 * Album title will be set to folder name when none is specified.
 * 
 * 0.2 2011-06-14
 * Added wildcard renaming
 * Added musicbrainz search
 * Added folder/file renaming
 * 
 * 0.1 2011-05-27 
 * Initial creation
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Diagnostics;
using System.IO;

namespace Common
{
  class Program
  {
    /// <summary>
    /// Holds version information
    /// for application
    /// </summary>
    static private string version = "0.3.0";

    /// <summary>
    /// Entry point for the client application.
    /// </summary>
    /// <param name="args"></param>
    /*[STAThread]
    static void Main(string[] args)
    {
      // Uncomment if proxy exists
      Trace.WriteLine("Starting proxy at 192.168.31.54:80");
      WebRequest.DefaultWebProxy = new WebProxy("http://192.168.31.54:80");
      WebRequest.DefaultWebProxy.Credentials = new NetworkCredential("James.McClure", "Hotwire03", "US");

      // Initialize logging
      Logger log = new Logger("mp3tag.log"); 

      // Run!
      RunClient();
      Console.ReadLine();
    }*/

    static void RunClient()
    {
      Trace.WriteLine("");
      Trace.WriteLine("***************************************************");
      Trace.WriteLine("Starting Application " + version);

      // Specify location of music folder
      File Root = new Release(@"C:\Users\James.McClure\My Music");
      //File Root = new Release(@"C:\Users\James.McClure\formats");

      // Process single songs
      foreach (Song song in Root.Songs)
      {
        Console.WriteLine("Song found " + song.Name);
      }

      // Process albums
      foreach (Release Release in Root.Releases)
      {
        Release.Collapse();

        // Perform search
        List<Metadata> Results = DispatchSearch(Release);

        if (Results.Count > 0)
        {
          // Prompt user
          Metadata Selection = Prompt(Release, Results);

          // Apply tag
          ApplyTag(Release, Selection);
        }

        // Album clean-up
        Results = null;
      }
    }

    static List<Metadata> DispatchSearch(File release)
    {
      List<Metadata> Results = new List<Metadata>();
      Results.AddRange(Search(new MusicBrainzService(release.Metadata, 1)));
      //Results.AddRange(Search(new LastfmSearch(Release, 1)));
      return Results;
    }

    static public List<Metadata> Search(WebService service)
    {
      List<Metadata> Results = service.Search();
      foreach (Metadata Collection in Results)
      {
        // Format tags first
        Formatter format = new Formatter();
        Collection.Format(format);
      }

      return Results;
    }

    static Metadata Prompt(File release, List<Metadata> results)
    {
      Metadata Selection = new MetadataCollection();

      // Present options to user (WPF)
      foreach (Metadata tag in results)
      {
        Console.WriteLine("\r\n[Track Count Match]: " + (tag.TrackCount == release.Count).ToString());
        Console.WriteLine(tag.Print() + "\r\n");
      }
      Console.Write("Enter Tag Selection [1-" + (results.Count()) + "]: ");

      // Check for valid numerical input
      string input = Console.ReadLine();
      int choice = int.Parse(int.TryParse(input, out choice) ? input : "99");
      
      switch (choice)
      {
        case 0:
          // Disable cache 
          release.Metadata.MusicBrainzReleaseId = null;
          results = DispatchSearch(release);
          Selection = Prompt(release, results);
          break;
        default:
          if (choice < 1 || choice > results.Count)
          {
            Selection = Prompt(release, results);
          }
          else
          {
            Selection = results.ElementAt(choice - 1);
          }
          break;
      }

      return Selection;
    }

    static public void ApplyTag(Release release, Metadata metadata)
    {
      // Set tag selection
        release.Metadata = metadata;

        // Format tag
        Formatter Formatter = new Formatter();
        release.Metadata.Format(Formatter);

        // Apply tag to file, download art & write checksum
        release.Publish();

        // File formatting
        Formatter = new Formatter();
        Formatter.SongCasing = Cases.LOWER;
        Formatter.FlatCasing = Cases.LOWER;
        Formatter.Add(" ", "_"); // easy to exchange spaces; should be checkbox option in gui
        release.Format(Formatter);

        // Rename files
        Renamer Renamer = new Renamer();
        Renamer.ReleaseFunction = "%releaseArtist%-%release%-%releaseYear%";
        Renamer.SongFunction = "%Track%-%Artist%-%Title%";
        Renamer.FlatFunction = "00-%releaseArtist%-%release%";
        release.Rename(Renamer);
    }
  }
}