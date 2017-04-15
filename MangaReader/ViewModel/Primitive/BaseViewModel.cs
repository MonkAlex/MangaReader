using System.Windows;
using MangaReader.UI.Services;

namespace MangaReader.ViewModel.Primitive
{
  public class BaseViewModel : NotifyPropertyChanged
  {
    public virtual void Load()
    {

    }

    public virtual void Unload()
    {

    }

    public virtual void Show()
    {

    }

    private void ViewOnUnloaded(object sender, RoutedEventArgs routedEventArgs)
    {
      Client.Dispatcher.InvokeAsync(this.Unload);
    }

    private void ViewOnLoaded(object sender, RoutedEventArgs routedEventArgs)
    {
      Client.Dispatcher.InvokeAsync(this.Load);
    }

    public static void SubToViewModel(FrameworkElement view)
    {
      view.DataContextChanged += (o, a) => ViewOnDataContextChanged(view, a);
    }

    private static void ViewOnDataContextChanged(FrameworkElement sender, DependencyPropertyChangedEventArgs args)
    {
      var model = args.OldValue as BaseViewModel;
      if (model != null)
      {
        sender.Loaded -= model.ViewOnLoaded;
        sender.Unloaded -= model.ViewOnUnloaded;
        if (sender.IsLoaded)
          model.ViewOnUnloaded(sender, new RoutedEventArgs());
        ViewService.Instance.TryRemove(model);
      }

      model = args.NewValue as BaseViewModel;
      if (model != null)
      {
        sender.Loaded += model.ViewOnLoaded;
        sender.Unloaded += model.ViewOnUnloaded;
        if (sender.IsLoaded)
          model.ViewOnLoaded(sender, new RoutedEventArgs());
        ViewService.Instance.AddOrReplace(model, sender);
      }

    }
  }
}