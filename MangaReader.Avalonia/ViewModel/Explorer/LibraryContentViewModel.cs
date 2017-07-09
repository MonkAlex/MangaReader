using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Threading;
using MangaReader.Core.Manga;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class LibraryContentViewModel : ViewModelBase
  {
    private ObservableCollection<IManga> items;

    public ObservableCollection<IManga> Items
    {
      get
      {
        if (items == null)
          RefreshItems();
        return items;
      }
      set { RaiseAndSetIfChanged(ref items, value); }
    }

    public void RefreshItems()
    {
      Dispatcher.UIThread.InvokeAsync(() =>
      {
        Items = new ObservableCollection<IManga>(Core.NHibernate.Repository.Get<IManga>().ToList());
      }, DispatcherPriority.ApplicationIdle);
    }
  }
}