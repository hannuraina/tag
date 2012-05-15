using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows;
using System.Collections.ObjectModel;
using Caliburn.Micro;
using tag.Framework;
using tag.Views;
using tag.ViewModels.Interfaces;
using tag.Data;

namespace tag.ViewModels
{
  [Export(typeof(IShell))]
  public class ShellViewModel : Conductor<IModule>.Collection.OneActive, IHandle<NavigationEvent>, IShell
  {
    #region Private Properties (Services)
    private IEventAggregator eventAggregator { get; set; }
    #endregion

    #region Public Properties (Bindable)
    private int currentState = 0;
    public int CurrentState {
      get { return currentState; } 
      set
      {
        if (value == currentState)
          return;
        currentState = value;
        NotifyOfPropertyChange(() => CurrentState);
      }
    }
    #endregion

    #region Action Guards
    
    #endregion

    #region Constructor
    [ImportingConstructor]
    public ShellViewModel([ImportMany]IEnumerable<Lazy<IModule, IModuleMetadata>> moduleHandles, IEventAggregator events)
    {
      // Initialize shell
      eventAggregator = events;
      eventAggregator.Subscribe(this);

      // Initialize VM
      var modules = from h in moduleHandles orderby h.Metadata.Order select h.Value;
      Items.AddRange(modules);
    }
    #endregion

    #region Override IHandle
    public void Handle(NavigationEvent e)
    {
      CurrentState = e.Target;
    }
    #endregion

    #region Override Conductor
    protected override void OnInitialize()
    {
      base.OnInitialize();
    }
    #endregion

    #region Public Actions

    #endregion
  }
}