using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using gui.Views;
using gui.Data;
using gui.ViewModels.Interfaces;
using Caliburn.Micro;
using Common;


namespace gui.ViewModels
{
  [Export(typeof(IModule))]
  [Export(typeof(LibraryViewModel))]
  [ExportMetadata("Order", 1)]
  public class LibraryViewModel : Screen, IModule
  {
    public string Description { get; private set; }

    # region Private Attributes (Services)
    private IEventAggregator eventAggregator { get; set; }
    #endregion

    #region Properties (Data)
    private File library { get; set; }
    public String LibraryPath
    {
      get { return library.Path; }
      set
      {
        if (library.Path == value)
          return;
        library.Path = value;

        NotifyOfPropertyChange(() => LibraryPath);
        NotifyOfPropertyChange(() => CanSkipLibrary);
      }
    }

    private File selectedItem;
    public File SelectedItem {
      get { return selectedItem; }
      set { 
        selectedItem = value; 
        NotifyOfPropertyChange(() => SelectedItem); 
      }
    }

    //this
    
    public BindableCollection<File> Releases
    {
      get { return new BindableCollection<File>(library.Releases); }
    }
    #endregion

    #region public actions guards
    public bool CanChangeLibrary {
      get { return true; }
    }

    // Could be updated later
    public bool CanSkipLibrary
    {
      get { return LibraryPath != null; }
    }
    #endregion

    [ImportingConstructor]
    public LibraryViewModel(IEventAggregator e)
    {
      eventAggregator = e;

      library = new Release();
      Description = "Initialize application";
      DisplayName = "Library";
    }

    #region public actions
    public void ChangeLibrary()
    {
      System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
      System.Windows.Forms.DialogResult result = dialog.ShowDialog();
      if (result == System.Windows.Forms.DialogResult.OK)
      {
        LibraryPath = dialog.SelectedPath;
      }
      LoadLibrary();
    }
    public void SkipLibrary()
    {
      eventAggregator.Publish(new NavigationEvent(State.Main));
    }
    
    // TODO: Move to background thread
    public void LoadLibrary()
    {
      library = new Release(LibraryPath);
    }
    #endregion
  }
}
