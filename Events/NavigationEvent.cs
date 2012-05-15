using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using tag.ViewModels;
using Caliburn.Micro;
using tag.Data;

namespace tag
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
