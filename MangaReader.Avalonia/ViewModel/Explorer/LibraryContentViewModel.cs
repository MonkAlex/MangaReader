using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using ReactiveUI;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class LibraryContentViewModel : ViewModelBase
  {
    private ObservableCollection<IManga> items;
    private string search;
    private IReactiveDerivedList<IManga> filteredItems;

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

    public IReactiveDerivedList<IManga> FilteredItems
    {
      get
      {
        if (filteredItems == null)
          RefreshItems();
        return filteredItems;
      }
      private set { RaiseAndSetIfChanged(ref filteredItems, value); }
    }

    public string Search
    {
      get { return search; }
      set
      {
        RaiseAndSetIfChanged(ref search, value);
        FilteredItems.Reset();
      }
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
        FilteredItems = Items.CreateDerivedCollection(
          x => x,
          Filter,
          (original, filtered) => string.Compare(original.Name, filtered.Name, StringComparison.InvariantCultureIgnoreCase));
      }, DispatcherPriority.ApplicationIdle);
    }

    private bool Filter(IManga manga)
    {
      if (manga == null)
        return false;

      if (Search == null)
        return true;
      
      return manga.Name.IndexOf(Search, StringComparison.InvariantCultureIgnoreCase) >= 0;
    }
  }
}