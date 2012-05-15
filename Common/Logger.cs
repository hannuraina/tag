/**
 * File: TraceLogger.cs
 * Author: James McClure
 * 
 * Extends trace listener functionality
 * to add timestamp information.
 * 
 * Revision History:
 * 0.1 2011-06-16
 * Initial Creation
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Common
{
  /// <summary>
  /// Adds timestamp logging to trace
  /// log files.
  /// </summary>
  public class Logger : TextWriterTraceListener
  {

    #region Ctors

    /// <summary>
    /// Default ctor. Sets up log file path.
    /// </summary>
    /// <param name="path">Location of log file.</param>
    public Logger(string path)
      : base(path) 
    {
      // Register logger
      Trace.Listeners.Add(this);
    }

    #endregion

    #region TextWriterTraceListener

    /// <summary>
    /// Prepends timestamp to all trace logging
    /// events
    /// </summary>
    /// <param name="message">string to log</param>
    public override void WriteLine(string message)
    {
      base.Write("[" + DateTime.Now.ToString() + "]:");
      base.Write(" ");
      base.Write(message);
      base.WriteLine("");
    }

    #endregion

  }
}
