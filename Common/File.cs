/**
 * File: Music.cs
 * Author: James McClure
 * 
 * Abstract class used to handle file manipulation 
 * and tagging.
 * 
 * Revision History:
 * 0.2.7 2011-07-18
 * Added logic for audio format conversion
 * 
 * 0.2.6 2011-06-14
 * Added Get method to support indexing on 
 * composite objects
 * 
 * 0.2.5 2011-06-10
 * Removed save and clear methods
 * 
 * 0.2.4 2011-06-09
 * Added childrenCount property
 * 
 * 0.2.3 2011-06-03
 * Added helper method exist to check if music
 * object still exists in folder heirarchy
 * 
 * 0.2.2 2011-06-02
 * Now clear existing tag prior to setting it
 * in order to delete unwanted frame values
 * Added public accessor for children
 * 
 * 0.2.1 2011-05-31 
 * Moved non-implemented methods to abstract class from 
 * concrete sub-classes.
 * Renamed collection methods AddChild & RemoveChild 
 * to Add() and Remove(), respectively.
 * 
 * 0.2 2011-05-30 
 * Cleaned up comments
 * Refactored some virtual methods in abstract methods.
 * 
 * 0.1 2011-05-27 
 * Initial creation jmcclure
 **/
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Caliburn.Micro;

namespace Common
{

  #region Enum

  public enum FileTypes
  {
    RELEASE = 0,
    SONG,
    FLAT
  }

  #endregion

  /// <summary>
  /// Class provides access to specified 
  /// system files and directories. Extends
  /// <see cref="IEnumerable<T>"/> to child classes.
  /// TODO: Change name of class that represents the data structure
  /// </summary>
  /// <remarks>
  /// Class is component actor in composite pattern. Object
  /// can refer to either Directory (Album) or File (Song).
  /// </remarks>
  public abstract class File : PropertyChangedBase
  {
    #region Attributes

    /// <summary>
    /// Full path of object.
    /// </summary>
    private string path;

    /// <summary>
    /// Extension of object.
    /// </summary>
    /// <remarks>Includes dot in value</remarks>
    private string extension;

    /// <summary>
    /// Holds abstract metadata reference for current object.
    /// </summary>
    /// <remarks>
    /// Could reference either <see cref="MetadataCollection"/> or <see cref="Id3"/>
    /// </remarks>
    private Metadata metadata;

    #endregion

    #region Properties

    /// <summary>
    /// Gets and sets the name of the of the current object.
    /// </summary>
    /// <remarks>
    /// Usually set automatically by <see cref="Music.Path"/> upon object creation.
    /// </remarks>
    public string Name { get; set; }

    /// <summary>
    /// Gets and sets current objects parent object in tree.
    /// </summary>
    /// <remarks>
    /// Root node has default parent of null. Auto-set
    /// depth when parent is set on file object.
    /// </remarks>
    public File Parent { get; set; }

    /// <summary>
    /// Gets and sets base file from the <see cref="TagLib.File"/> library.
    /// </summary>
    /// <remarks>
    /// This object provides the interface for actually writing/reading
    /// metadata to and from the objects. After setting the local <see cref="Music.file"/>
    /// reference, the local abstract <see cref="Tag"/> is set for updating and formatting
    /// </remarks>
    public TagLib.File Base { get; set; }

    /// <summary>
    /// Stores current objects depth in the tree.
    /// </summary>
    /// <remarks>
    /// See <see cref="Music.depth"/> for explanation on depth referencing.
    /// </remarks>
    public int Depth { get; set; }

    /// <summary>
    /// Gets and sets formatter last applied
    /// to current object.
    /// </summary>
    public Formatter Formatter { get; set; }

    /// <summary>
    /// Gets and set absolute path of current object. 
    /// </summary>
    /// <remarks> 
    /// Auto sets public properties <see cref="Music.Name"/> and <see cref="Music.Extension"/>
    /// </remarks>
    public string Path
    {
      get { return path; }
      set
      {
        path = value;
        
        // Do not set extension for extensions
        if (!System.IO.Directory.Exists(path))
        {
          Name = System.IO.Path.GetFileNameWithoutExtension(value);
          Extension = System.IO.Path.GetExtension(value);
        }

        // Get full directory name
        else
        {
          Name = System.IO.Path.GetFileName(value);
        }
      }
    }

    /// <summary>
    /// Stores file extension of current object.
    /// </summary>
    /// <remarks>
    /// Removed from filename prior to renaming/tagging.
    /// String contains dot (.) char. All extensions 
    /// are lower case. 
    /// </remarks>
    public string Extension
    {
      get { return extension; }
      set { extension = value.ToLower(); }
    }

    /// <summary>
    /// Gets and sets current object's metdata <see cref="Tag"/>.
    /// </summary>
    /// <remarks>
    /// After setting updating the associated abstract tag
    /// the TagFile.File <see cref="Music.file"/> base file
    /// is updated via the extension medthod <see cref="TagLibExtension.Set"/>.
    /// This class provides a method to create/update and format tags
    /// prior to writing to the base file. 
    /// 
    /// Added check for music files so placeholder tag could be applied
    /// to art files for renaming/formatting.
    /// </remarks>
    public virtual Metadata Metadata
    {
      get { return metadata; }
      set
      {
        metadata = value;

        // Only apply tag to music files
        if (Song.SongTypes.Contains(Extension))
        {
          Base.Tag.Clear();
          Base.Tag(metadata);
        }
      }
    }

    /// <summary>
    /// Returns the type of the current object.
    /// </summary>
    public FileTypes FileType
    {
      get
      {
        if (Song.SongTypes.Contains(Extension))
          return FileTypes.SONG;
        if (Extension == String.Empty || Extension == null)
          return FileTypes.RELEASE;
        return FileTypes.FLAT;
      }
    }

    /// <summary>
    /// Stores the type of encoding to use when
    /// processing files.
    /// </summary>
    public Encoding TargetFormat { get; set; }

    public virtual IEnumerable<File> Songs
    {
      get { yield return this; }
    }

    public virtual IEnumerable<File> Releases
    {
      get { yield return this; }
    }

    #endregion

    #region Virtual

    /// <summary>
    /// Moves current object node up in tree heirarchy.
    /// TODO: Add ability to move either up or down, Move(-2)
    /// the tree heirarchy.
    /// </summary>
    /// <param name=delta>
    /// Number of levels in which to move node.
    /// </param>
    /// <example>
    /// CurrentLevel = /root/foo/foo1/foo2/object
    /// Move(2);
    /// CurrentLevel = /root/foo/object
    /// </example>
    /// <remarks>
    /// <see cref="Music.depth"/> and <see cref="Music.Parent"/>
    /// will be updated to reflect the nodes new standing. Note,
    /// the child node will not be deleted from the original parent's 
    /// child listing.
    /// </remarks>
    public virtual void Move(int delta)
    {
      // No movement
      if (delta == 0)
      {
        return;
      }

      // Increment through parent obejects until
      // specified delta-1 level is found.
      for (var CurrentLevel = 0; CurrentLevel < delta; CurrentLevel++)
      {
        Parent = Parent.Parent;
      }

      // Move file to specified level in heriarchy, assuming
      // it does not already exist.
      String Destination = Parent.Path + "\\" + Name + Extension;
      if (!System.IO.File.Exists(Destination))
      {
        Trace.WriteLine("Moving file " + Name + " to " + Parent.Path);

        System.IO.File.Move(Path, Destination);
        Path = Parent.Path + "\\" + Name + Extension;

        // Add current node to new parents children listing.
        Parent.Add(this);

        // Recreate base file. Can not explicitly set new location
        if (Song.SongTypes.Contains(Extension))
        {
          Base = TagLib.File.Create(Path);
        }
      }
      // Delete object node in case of duplicates.
      else
      {
        Delete();
      }
    }

    /// <summary>
    /// Formats current object's tag which is the basis for renaming functionality.
    /// </summary>
    /// <param name="format">Pattern to format object's name with</param>
    /// <param name="childFormat">Format to apply to children, if they exist</param>
    /// <remarks>
    /// childFormat allows the user to specify different formats for
    /// parent and child objects. Instead of formatting the Name attribute
    /// the tag is formatted so the user can decide on which attributes 
    /// are used in the renaming process.
    /// </remarks>
    public virtual void Format(Formatter formatter)
    {
      Formatter = formatter;
      Name = formatter.Format(FileType, Name);
    }

    /// <summary>
    /// Abstract method to rename private field <see cref="Music.name"/>
    /// </summary>
    /// <remarks>
    /// See <see cref="Song.Flatten()"/> and <see cref="Release.Flatten()"/> for implementation.
    /// </remarks>
    public virtual void Rename(Renamer renamer)
    {
      Name = renamer.Rename(this);
      if (Formatter != null)
      {
        Name = Formatter.Format(FileType, Name);
      }
      string NewPath = Parent.Path + "\\" + Name + Extension;
      System.IO.File.Move(Path, NewPath);
      Path = NewPath;
    }

    /// <summary>
    /// Base method for deleting current object from parent's child listing.
    /// </summary>
    public virtual void Delete()
    {
      // Remove parent pointer if one exists,
      // undeclared nodes will not have a parent
      if (Parent != null)
      {
        Parent.Remove(this);
      }
    }

    /// <summary>
    /// Returns object at specified index.
    /// </summary>
    /// <param name="index">Index of object to return</param>
    /// <returns>Object at specified index</returns>
    /// <remarks>
    /// Leaf objects will return themselves
    /// </remarks>
    public virtual File Get(int index)
    {
      return this;
    }

    /// <summary>
    /// Gets number of children contained in current object.
    /// Defaults to 1.
    /// </summary>
    public virtual int Count
    {
      get { return 1; }
    }

    /// <summary>
    /// Gets and sets child collection for the current
    /// object.
    /// </summary>
    /// <remarks>
    /// Only implemented by composite classes.
    /// </remarks>
    public virtual List<File> Children
    {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Remove specified item from child collection.
    /// </summary>
    /// <param name="Item">Item to be removed to collection</param>
    /// <remarks>
    /// Only implemented by composite extension. 
    /// </remarks>
    public virtual void Remove(File file)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Download/generate non-audio files associated with
    /// the current object.
    /// </summary>
    public virtual void Publish()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Add item to child collection if one exists
    /// </summary>
    /// <param name="Item">Item to be added to collection</param>
    /// <remarks>
    /// Only implemented by composite extension. 
    /// </remarks>
    public virtual void Add(File file)
    {
      throw new NotImplementedException();
    }

    #endregion

    #region Abstract

    /// <summary>
    /// Abstract method which moves all leaf nodes to highest parent
    /// level below root.
    /// </summary>
    /// <remarks>
    /// See <see cref="Song.Flatten()"/> and <see cref="Release.Flatten()"/> for implementation.
    /// </remarks>
    public abstract void Collapse();

    /// <summary>
    /// Checks whether the current object's base file
    /// reference exists.
    /// </summary>
    /// <returns>True if it exists, false otherwise</returns>
    public abstract bool Exists();

    #endregion

    #region IEnumerable

    /// <summary>
    /// Provides enumeration over current objects generic <see cref="Music"/>
    /// child listing.
    /// </summary>
    /// <returns>Generic enumerator for Music objects</returns>
    //public virtual IEnumerator<File> GetEnumerator()
    //{
    //  yield return this;
    //}


    /// <summary>
    /// Provides base enumeration interface for current object.
    /// </summary>
    /// <returns>List enumerator for child objects</returns>
    /// <remarks>
    /// Calls generic method <see cref="IEnumerator<Music> GetEnumerator"/> to actually
    /// provide enumeration
    /// </remarks>
    //IEnumerator IEnumerable.GetEnumerator()
    //{
    //  return GetEnumerator();
    //}

    #endregion
  }
}
