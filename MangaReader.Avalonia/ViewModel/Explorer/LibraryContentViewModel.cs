using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;

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

    public async Task RefreshItems()
    {
      while (!Core.NHibernate.Mapping.Initialized)
      {
        Log.Add("Wait nhibernate initialization...");
        await Task.Delay(500);
      }
      Dispatcher.UIThread.InvokeAsync(() =>
      {
        Items = new ObservableCollection<IManga>(Core.NHibernate.Repository.Get<IManga>().ToList());
      }, DispatcherPriority.ApplicationIdle);
    }
  }
}