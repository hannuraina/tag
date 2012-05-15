/**
 * File: Album.cs
 * Author: James McClure
 * 
 * Provides implementaton for the abstract class Music.
 * Represents a directory in the file heirarchy.
 * 
 * Revision History:
 * 0.2.4 2011-07-18
 * Added logic for audio format conversion
 * 
 * 0.2.3 2011-06-23
 * Added checksum functionality
 * 
 * 0.2.2 2011-06-14
 * Added Get method for child indexing
 * Added most recent formatter attribute
 * to allow formatting prior to renaming
 * 
 * 0.2.1 2011-06-10
 * Removed save and clear methods
 * 
 * 0.2.4 2011-06-09
 * Fixed bug with empty album art urls
 * 
 * 0.2.3 2011-06-03
 * Added helper method exists
 * Added logic for downloading album art and storing
 * in album tags
 * 
 * 0.2.2 2011-06-02
 * Fixed bug where Tag property was crashing at runtime
 * once called.
 * Added public accessor for children
 * Added sort on children list after insertion
 * 
 * 0.2.1 2011-05-31
 * Renamed AddChild() & RemoveChild() to Add() and Remove()
 * respectively.
 * 
 * 0.2 2011-05-30 
 * No longer cache children tag objects
 * Cleaned up comments
 * 
 * 0.1 2011-05-27 
 * Initial creation
 **/
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Text;

namespace Common
{
  /// <summary>
  /// Class implements <see cref="Music"/> and takes the form 
  /// of a directory, or file of files, in the current context.
  /// </summary>
  /// <remarks>
  /// Portrays the composite in the composite design pattern along
  /// with <see cref="Music"/> and <see cref="Song"> (leaf). The children or 
  /// leafs are recognized as a List<Music> generic. So albums
  /// could contain both albums and songs.
  /// </remarks>
  /// <seealso cref="Music"/>
  public class Release : File
  {

    #region Attributes

    /// <summary>
    /// Stores the list of music objects the reside 
    /// underneath the current node in the tree heirarchy.
    /// </summary>
    /// <remarks>
    /// Children can contain <see cref="Release"/> typed objects.
    /// </remarks>
    private List<File> children;

    /// <summary>
    /// Stores list of metadata associated with
    /// child files.
    /// </summary>
    private Metadata metadata;

    #endregion

    #region Properties

    /// <summary>
    /// Gets and sets the metadata for the current album.
    /// </summary> 
    /// <exmaple>
    /// // returns artist stored in first song
    /// string AlbumArtist = Album.Tag.Artist;
    /// </exmaple>
    /// <remarks>
    /// Because this is a composite class get and set operations
    /// are ultimately implemented by the leaf object <see cref="Song"/>.
    /// For gets a dyanmic <see cref="MetadataCollection"/> list is created
    /// from child tag objects. This way any intermediate changes made
    /// directly to a child will be picked up by the parent.
    /// Set iterates over that same child listing
    /// setting each individual tag in turn.
    /// </remarks>
    public override Metadata Metadata
    {
      get { return metadata; }
      set
      {
        Art.Metadata = value.First();
        Checksum.Metadata = value.First();
        metadata = value;
      }
    }

    /// <summary>
    /// Gets number of audio files contained in current object.
    /// Defaults to 1.
    /// </summary>
    public override int Count
    {
      get { return children.Count(c => Song.SongTypes.Contains(c.Extension)); }
    }

    /// <summary>
    /// Gets and sets album art for the current
    /// object
    /// </summary>
    public Art Art { get; set; }

    /// <summary>
    /// Gets and sets the checksum function
    /// that will be applied.
    /// </summary>
    public Checksum Checksum { get; set; }

    /// <summary>
    /// Provides iterator for song files in current
    /// directory
    /// </summary>
    public override IEnumerable<File> Songs
    {
      get
      {
        return (from s in children 
                where s.FileType == FileTypes.SONG
                select s).AsEnumerable();
      }
    }

    /// <summary>
    /// Provides iterator for song files in current
    /// directory
    /// </summary>
    public override IEnumerable<File> Releases
    {
      get
      {
        return (from r in children
                where r.FileType == FileTypes.RELEASE
                select r).AsEnumerable();
      }
    }

    private bool Root
    {
      get { return Depth == 0; }
    }

    #endregion
    
    #region Constructors

    /// <summary>
    /// Default ctor
    /// </summary>
    public Release() 
    {
      Art = new Art();
      TargetFormat = Encoding.MP3;
      Checksum = new Md5();
      children = new List<File>();
      metadata = new MetadataCollection();
    }

    /// <summary>
    /// Creates new tree heirarchy underneath the specified path. 
    /// Leaf nodes are represented by <creg="Song"/> and sub-directories
    /// by <see cref="Album"/>.
    /// </summary>
    /// <param name="depth">Number of levels down object resides from root</param>
    /// <param name="path">Absolute path of the object. Must be a directory.</param>
    /// <remarks>
    /// Depth defaults to 0, meaning root. Each child's depth is incremented by 1
    /// and the child's parent is set to the current object. The album object
    /// assumed the path represents a directory, otherwise a <see cref="Song"/> 
    /// should be created.
    /// 
    /// Once individual files have been read in the metadata for the 
    /// folder is set since requesting Metadata.Artist pulls the artist
    /// tag from the first file in the folder.
    /// </remarks>
    /// <exception cref="IOException">When path is not directory</exception>
    public Release(string path, int depth=0)
    {
      // Initialize album parameters
      Path = path;
      Depth = depth;
      Art = new Art();
      TargetFormat = Encoding.MP3;
      Checksum = new Md5();
      children = new List<File>();
      metadata = new MetadataCollection();

      // Parse directories
      foreach (var Item in System.IO.Directory.GetDirectories(Path))
      {
        Add(new Release(Item, Depth + 1));
      }

      // Parse individual files
      foreach (var Item in System.IO.Directory.GetFiles(Path))
      {
        Add(new Song(Item, Depth + 1));
      }

      if (!Root)
      {
        // If no album metadata set, use folder name
        Metadata.Release = Metadata.Release ?? Name;
      }
    }

    /// <summary>
    /// Provides implicit casting from a path string to an <see cref="Release"/>
    /// </summary>
    /// <param name="path">Absolute path of the directory</param>
    /// <returns>New album object</returns>
    static public implicit operator Release(string path)
    {
      return new Release(path);
    }

    #endregion

    #region File

    /// <summary>
    /// Remove specified music object from the current objects
    /// child listing. Updates parent of the child object
    /// prior to removal.
    /// </summary>
    /// <param name="Item">Music object to be removed</param>
    public override void Remove(File file)
    {
      // clear parent reference
      file.Parent = null;
      children.Remove(file);
    }

    /// <summary>
    /// Iterate tree heirarchy and move leaf files to highest level below 
    /// root. Sub-directories are deleted from the heirarchy once
    /// all children (both files and folders) have been moved or deleted.
    /// </summary>
    /// <remarks>
    /// Highest level below root in the context of depth would be
    /// considered 1 <see cref="Enum.Scope"/>. In keeping
    /// with the composite design primary implementation is left
    /// to the lead node <see cref="Song"/>. C# does not allow elements of a list
    /// to be changed during iteration, therefore we iterate over a cahced version
    /// of the child listing.
    /// </remarks>
    public override void Collapse()
    {
      // Allow for child iteration after child deletion.
      List<File> files = new List<File>(children);
      foreach (File File in files)
      {
        File.Collapse();
      }
  
      // Delete self only if
      // not root-level directory
      if (Depth > (int)Scope.LEVEL1)
      {
        Delete();
      }
    }

    /// <summary>
    /// Deletes current object and the disk file
    /// it represents.
    /// </summary>
    /// Calls base method <see cref="Music.Delete()"/>
    /// to handle class based actions like updating the parent
    /// with the new child listing.
    public override void Delete()
    {
      Trace.WriteLine("Deleting " + Path);
      base.Delete();
      System.IO.Directory.Delete(Path);
    }

    /// <summary>
    /// Formats current object's name with the specified formmating pattern.
    /// </summary>
    /// <param name="format">Type of format to apply</param>
    /// <param name="childFormat">Type of format to apply to children object if any exist</param>
    /// <remarks>
    /// In case the user wants to format parent and child objects differently,
    /// the childFormat parameter is provided. If not provided the children
    /// will use the parent's format. If a childformat is not specified
    /// the parent's format is used by default.
    /// </remarks>
    public override void Format(Formatter formatter)
    {
      Trace.WriteLine("Formatting album " + Name);

      // Store most recent formatter for use when renaming
      Formatter = formatter;

      foreach (File child in children)
      {
        child.Format(formatter);
      }

      // Rename folder
      Name = formatter.Format(FileType, Name);
    }

    /// <summary>
    /// Renames children objects physical files with string
    /// stored in associated Name field.
    /// </summary>
    /// <remarks>
    /// Children objects are renamed first followed by the current object
    /// to avoid <see cref="FileNotFoundException"/>
    /// </remarks>
    public override void Rename(Renamer renamer)
    {
      Trace.WriteLine("Renaming album " + Name);

      // Apply child renaming
      foreach (File child in children)
      {
         child.Rename(renamer);
      }

      // Re-apply any formatting after rename
      Name = renamer.Rename(this);
      if (Formatter != null)
      {
        Name = Formatter.Format(FileType, Name);
      }

      // Case-sensitive based renames fail under Windows. Workaround,
      // First change name by appending character, then rename to new name with 
      // corrected case.
      string NewPath = Parent.Path + "\\" + Name + "_";
      System.IO.Directory.Move(Path, NewPath);
      System.IO.Directory.Move(NewPath, NewPath.TrimEnd(new char[] { '_' }));
    }

    /// <summary>
    /// Checks whether directory exists
    /// </summary>
    /// <returns>True if exists, false otherwise</returns>
    public override bool Exists()
    {
      return System.IO.Directory.Exists(Path);
    }

    /// <summary>
    /// Returns object at specified index.
    /// </summary>
    /// <param name="index">Index of object to return</param>
    /// <returns>Object at specified index</returns>
    /// <remarks>
    /// Leaf objects will return themselves. Throws index
    /// out of range exception.
    /// </remarks>
    public override File Get(int index)
    {
      if (children.Count() > 0 && index <= children.Count())
      {
        return children.ElementAt(index);
      }
      else
      {
        throw new IndexOutOfRangeException();
      }
    }

    /// <summary>
    /// Adds new child object to the current object, in turn
    /// incrementing the depth of the item and setting the child's
    /// parent to the current object.
    /// </summary>
    /// <param name="item">Music object to add as child</param>
    /// <remarks>
    /// Added objects are appended to the List<Music> children object.
    /// Children are added according to track number stored in 
    /// the objects tag. Directories are always weighted 
    /// zero.
    /// </remarks>
    /// <see also="mp3tag.MusicComparer"/>
    public override void Add(File file)
    {
      if (!file.Exists())
      {
        return;
      }

      Metadata.Add(file.Metadata);
      file.Parent = this;
      
      // Parent has been initialized and item is not being duplicated
      if (Name != null && !children.Contains(file))
      {
        // After child is added sort by track number
        children.Add(file);
        children.Sort(new FileComparer(MusicCompareTypes.TRACKNUMBER));
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Writes additional files to file system including art
    /// and checksum files. Applies metadata to files.
    /// </summary>
    public override void Publish()
    {
      // Download art first so it's checksum can
      // be calculated.
      Add(Art); 
      Art.Download();

      for (int index = 0; index < Songs.Count(); index++)
      {
        File Child = Songs.ElementAt(index);
        StringBuilder Buffer = Checksum.Hash(Child);         // Generate file hash
        Child.Metadata.Comment = Buffer.ToString();          // Append checksum to comments field of tag
        Child.Metadata = Metadata.Get(index);                // Apply tag
        Child.Base.SetImage(Art);                            // Embed album art into tag
      }

      // Generate checksum before adding to track
      Add(Checksum);
      Checksum.Generate();
    }

    #endregion
  }
}
