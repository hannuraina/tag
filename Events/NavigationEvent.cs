using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gui.ViewModels;
using Caliburn.Micro;
using gui.Data;

namespace gui
{

  public class NavigationEvent
  {
    public int Target { get; private set; }

    public NavigationEvent(State target)
    {
      Target = (int)target;
    }
  }
}
