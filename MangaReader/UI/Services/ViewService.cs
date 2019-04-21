using System;
using System.Windows;
using MangaReader.Core.Services;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.UI.Services
{
  public class ViewService : SimpleDictionary<BaseViewModel, FrameworkElement>
  {
    private static Lazy<ViewService> instance = new Lazy<ViewService>(() => new ViewService());

    public static ViewService Instance { get { return instance.Value; } }

    public T TryGet<T>(BaseViewModel key) where T : FrameworkElement
    {
      var value = base.TryGet(key) as T;
      if (value == null)
      {
        var windowType = ViewResolver.Instance.TryGet(key.GetType());
        if (windowType != null)
        {
          value = (T)Activator.CreateInstance(windowType);
          value.DataContext = key;
          AddOrReplace(key, value);
        }
        else
        {
          Log.AddFormat("Not resolve {0} type.", key.GetType().FullName);
        }
      }
      return value;
    }
  }
}