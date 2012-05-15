using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tag.ViewModels;

namespace tag
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
