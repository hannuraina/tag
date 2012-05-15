using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gui.ViewModels;

namespace gui
{

  public class StatusEvent
  {
    public String Status { get; set; }

    public StatusEvent(String status)
    {
      Status = status;
    } 
  }
}
