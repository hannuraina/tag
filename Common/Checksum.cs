/**
 * File: Checksum.cs
 * Author: James McClure
 * 
 * Abstract class for checksum computation
 * of files.
 * 
 * Revision History:
 * 0.1 2011-06-27
 * Initial Creation
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
  /// <summary>
  /// Calculates checksum values for specified
  /// files.
  /// </summary>
  public abstract class Checksum : File
  {

    #region Properties

    /// <summary>
    /// Gets and sets current hash value
    /// </summary>
    public StringBuilder Buffer { get; set; }

    #endregion

    #region Abstract

    /// <summary>
    /// Calculates hash value of file based on the 
    /// concrete checksum class.
    /// </summary>
    /// <param name="file">File to calculate the hash for</param>
    /// <returns>String hash representation</returns>
    public abstract StringBuilder Hash(File file);
    
    /// <summary>
    /// Writes hashed values to file
    /// </summary>
    public abstract void Generate();

    #endregion

    #region File

    /// <summary>
    /// Checks whether or not the hash files
    /// has been created
    /// </summary>
    /// <returns>True if file exists, false otherwise</returns>
    public override bool Exists()
    {
      return Buffer != null || Buffer.ToString() != String.Empty;
    }

    /// <summary>
    /// Not implemented
    /// </summary>
    public override void Collapse()
    {
      throw new NotImplementedException();
    }

    #endregion

  }
}