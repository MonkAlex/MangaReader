using System;
using System.Linq;
using System.Windows;
using MangaReader.Core.Services;
using MangaReader.ViewModel;

namespace MangaReader.Services
{
  public static class WindowHelper
  {
    public static LibraryViewModel Library
    {
      get
      {
        var fe = WindowModel.Instance.Content as FrameworkElement;
        if (fe != null)
        {
          var pageModel = fe.DataContext as MainPageModel;
          if (pageModel != null)
            return pageModel.Library;
          else
          {
            Log.Add("Неудачная попытка получить библиотеку, не распознан контекст.");
          }
        }
        else
        {
          Log.Add("Неудачная попытка получить библиотеку, не найдено содержимое основного окна.");
        }
        return null;
      }
    }

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