using System;
using System.Windows;
using MangaReader.Core.Services;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.UI.Services
{
  public class ViewService : SimpleDictionary<BaseViewModel, Window>
  {
    private static Lazy<ViewService> instance = new Lazy<ViewService>(() => new ViewService());

    public static ViewService Instance { get { return instance.Value; } }

    public override Window TryGet(BaseViewModel key)
    {
      var value = base.TryGet(key);
      if (value == null)
      {
        var windowType = ViewResolver.Instance.TryGet(key.GetType());
        if (windowType != null)
        {
          value = (Window) Activator.CreateInstance(windowType);
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