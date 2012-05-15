using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;

namespace gui.ViewModels
{
  public class FooterViewModel : PropertyChangedBase, IHandle<StatusEvent>
  {
    #region Private Properties (Services)
    private IEventAggregator eventAggregator { get; set; }
    private String status;
    #endregion

    #region Public Properties (Binding)
    public String Status
    {
      get { return status; }
      set
      {
        status = value.ToUpper();
        NotifyOfPropertyChange(() => Status);
      }
    }
    #endregion

    #region Constructor
    public FooterViewModel(IEventAggregator events)
    {
      eventAggregator = events;
    }
    #endregion

    #region override IHandle
    public void Handle(StatusEvent e)
    {
      Status = e.Status;
    }
    #endregion
  }
}