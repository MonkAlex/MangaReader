using System;
using System.Linq;
using System.Windows;

namespace MangaReader.Services
{
  public static class WindowHelper
  {
    public static Window Owner
    {
      get
      {
        Func<Window> action = () => Application.Current.Windows.Cast<Window>()
          .Where(w => w.IsLoaded)
          .OrderBy(w => w.IsActive).LastOrDefault();
        if (Application.Current.CheckAccess())
          return action();
        else
          return Client.Dispatcher.Invoke(action);
      }
    }
  }
}