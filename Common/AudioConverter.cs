/**
 * File: AudioConverter.cs
 * Author: James McClure
 * 
 * Converts various audio formats
 * to other formats as specified
 * by the user.
 * 
 * Revision History:
 * 0.2 2011-07-19
 * Conversion of .wmv works by default. Have
 * to add codec pack to get m4a, etc functioning
 * 
 * 0.1 2011-06-16
 * Initial Creation
 **/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Alvas.Audio;

namespace Common
{

  #region Enum

  /// <summary>
  /// Stores types of formats 
  /// that will be supported.
  /// </summary>
  public enum Encoding
  {
    MP3=0,
    FLAC,
    WAV,
    M4A
  };

  #endregion

  /// <summary>
  /// Converts between different audio formats.
  /// </summary>
  static public class AudioConverter
  {
    static public string Convert(File file, Encoding targetEnc)
    {
      Trace.WriteLine("Converting " + file.Name + file.Extension + " to " + targetEnc.ToString("G"));

      // Read in file & formatting
      IAudioReader Reader = new DsReader(file.Path);
      IntPtr SourceFormat = Reader.ReadFormat();
      IntPtr TargetFormat = AudioCompressionManager.GetCompatibleFormat(SourceFormat, AudioCompressionManager.MpegLayer3FormatTag);
      
      // Create new file
      byte[] Data = Reader.ReadData();
      byte[] Output = AudioCompressionManager.Convert(SourceFormat, TargetFormat, Data, true);
      string Filename = Path.GetDirectoryName(file.Path) + "\\" + file.Name + "." + targetEnc.ToString("G").ToLower();
      BinaryWriter Writer = new BinaryWriter(System.IO.File.Create(Filename));
      Writer.Write(Output, 0, Output.Length);

      // Cleanup & remove source
      Reader.Close();
      Writer.Close();
      file.Delete();

      return Filename;
    }
  }
}