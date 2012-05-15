using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using tag.ViewModels.Interfaces;
using tag.Data;

namespace tag.ViewModels
{
  [Export(typeof(IModule))]
  [ExportMetadata("Order", 2)]
  public class MainViewModel : Screen, IModule
  {
    #region Private Attributes (Services)
    private IEventAggregator eventAggregator;
    #endregion

    #region Public Properties (Data)
    public String Description { get; private set; }
    #endregion

    #region Public Properties (Shared VM)
    [Import]
    public LibraryViewModel LibraryVM { get; private set; }
    #endregion

    #region IHandle

    #endregion

    #region Screen
    protected override void OnInitialize()
    {
      base.OnInitialize();
    }
    #endregion
   
    #region Constructors
    [ImportingConstructor]
    public MainViewModel(IEventAggregator events)
    {
      eventAggregator = events;
      eventAggregator.Subscribe(this);

      Description = "Main Library View";
      DisplayName = "Main";
    }
    #endregion

    #region Public Actions
    
    #endregion

  }
}
