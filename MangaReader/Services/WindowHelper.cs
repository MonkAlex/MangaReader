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
        return Application.Current.Windows.Cast<Window>()
          .Where(w => w.IsLoaded)
          .OrderBy(w => w.IsActive).LastOrDefault();
      }
    }
  }
}