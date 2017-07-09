using System.Collections.ObjectModel;
using MangaReader.Core.Manga;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class SearchContentViewModel : ViewModelBase
  {
    private ObservableCollection<IManga> items;
    private string search;

    public string Search
    {
      get { return search; }
      set { RaiseAndSetIfChanged(ref search, value); }
    }

    public ObservableCollection<IManga> Items
    {
      get { return items; }
      set { RaiseAndSetIfChanged(ref items, value); }
    }

    public SearchContentViewModel()
    {
      this.items = new ObservableCollection<IManga>();
    }
  }
}