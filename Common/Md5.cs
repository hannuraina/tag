/**
 * File: Md5.cs
 * Author: James McClure
 * 
 * Calculates checksum value for specified
 * file.
 * 
 * Revision History:
 * 0.1 2011-06-27
 * Initial Creation
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Common
{

  /// <summary>
  /// Calculates checksum value for files 
  /// using md5 hashing function.
  /// </summary>
  class Md5 : Checksum
  {

    #region Ctor

    /// <summary>
    /// Default ctor
    /// </summary>
    public Md5()
    {
      Buffer = new StringBuilder();
      Metadata = new Id3();
      Extension = ".md5";
    }

    #endregion

    #region Checksum

    /// <summary>
    /// Calculate hashed checksum of file based
    /// on md5 hashing function.
    /// </summary>
    /// <param name="file">File to calculate hash of</param>
    /// <returns>String-based representation of hash</returns>
    public override StringBuilder Hash(File file)
    {
      StringBuilder hash = new StringBuilder();
      MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
      FileStream stream = new FileStream(file.Path, FileMode.Open, FileAccess.Read, FileShare.Read, 8192);
      md5.ComputeHash(stream);

      foreach (byte h in md5.Hash)
      {
        hash.Append(String.Format("{0:X2}", h));
      }
      stream.Close();
      Buffer.Append(hash + " !" + file.Name + "\r\n");
      return hash;
    }

    /// <summary>
    /// Creates file containing hash data
    /// </summary>
    public override void Generate()
    {
      Path = Parent.Path + "\\checksum" + Extension;
      System.IO.File.WriteAllText(Path, Buffer.ToString());
    }

    #endregion

  }
}
