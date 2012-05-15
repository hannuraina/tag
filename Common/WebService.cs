/**
 * File: WebService.cs
 * Author: James McClure
 * 
 * Interface for web service searches.
 * 
 * Revision History:
 * 0.1.1 2011-06-08
 * Adapter to abstract class.
 * Added tokens dictionary to base scope.
 * 
 * 0.1 2011-06-07
 * File creation
 **/
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Common
{

  /// <summary>
  /// Provides interface for web searches for metadata.
  /// </summary>
  /// <remarks>
  /// Implemented by amazon, lastfm, itunes etc.
  /// </remarks>
  abstract public class WebService
  {

    #region Attributes

    /// <summary>
    /// Stores maximum number of results
    /// to be returned by search query
    /// </summary>
    private int results;

    #endregion

    #region Properties

    /// <summary>
    /// Gets and sets tokens to be searched.
    /// </summary>
    /// <remarks>
    /// Values obtained from search <see cref="mp3tag.Metadata"/>
    /// </remarks>
    public Dictionary<string, string> Tokens { get; set; }

    /// <summary>
    /// Gets and sets number of results to return from
    /// each search query
    /// </summary>
    /// <remarks>
    /// Defaults to 5
    /// </remarks>
    public int MaxResults
    {
      get { return results; }
      set
      {
        if (value <= 0 || value >= 5)
        {
          results = 5;
        }
        else
        {
          results = value;
        }
      }
    }

    /// <summary>
    /// Gets and sets the file or files
    /// being searched for.
    /// </summary>
    public Metadata Metadata { get; set; }

    #endregion

    #region Abstract

    /// <summary>
    /// Search interface implemented by various
    /// web services.
    /// </summary>
    /// <returns>List of tags collections representing album tags</returns>
    public abstract List<Metadata> Search();

    #endregion

  }
}
