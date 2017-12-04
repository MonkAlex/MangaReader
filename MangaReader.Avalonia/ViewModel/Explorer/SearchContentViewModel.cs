using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MangaReader.Avalonia.ViewModel.Command;
using MangaReader.Core.Manga;
using MangaReader.Core.Services.Config;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class SearchContentViewModel : ViewModelBase
  {
    private ObservableCollection<MangaViewModel> items;
    private string search;
    private DelegateCommand startSearch;
    private string manualUri;
    private DelegateCommand addManual;

    public string Search
    {
      get { return search; }
      set { RaiseAndSetIfChanged(ref search, value); }
    }
    
    public DelegateCommand StartSearch
    {
      get { return startSearch; }
      set { RaiseAndSetIfChanged(ref startSearch, value); }
    }

    public ObservableCollection<MangaViewModel> Items
    {
      get { return items; }
      set { RaiseAndSetIfChanged(ref items, value); }
    }

    public string ManualUri
    {
      get { return manualUri; }
      set { RaiseAndSetIfChanged(ref manualUri, value); }
    }

    public DelegateCommand AddManual
    {
      get { return addManual; }
      set { RaiseAndSetIfChanged(ref addManual, value); }
    }

    private void UpdateManga()
    {
      Items.Clear();
      foreach (var manga in ConfigStorage.Plugins.SelectMany(p => p.GetParser().Search(Search)))
      {
        if (Items.All(i => i.Uri != manga.Uri))
          Items.Add(new MangaViewModel(manga));
      }
    }

    private void AddManga()
    {
      #warning Отсюда нужен переход к превью.
      Mangas.CreateFromWeb(new Uri(ManualUri));
    }

    public SearchContentViewModel()
    {
      this.Items = new ObservableCollection<MangaViewModel>();
      this.StartSearch = new DelegateCommand(UpdateManga, () => !string.IsNullOrWhiteSpace(Search)) {Name = "Search"};
      this.AddManual = new DelegateCommand(AddManga, () => !string.IsNullOrWhiteSpace(ManualUri)) {Name = "Add"};
      this.PropertyChanged += (sender, args) =>
      {
        if (args.PropertyName == nameof(Search))
          this.StartSearch.OnCanExecuteChanged();
        if (args.PropertyName == nameof(ManualUri))
          this.AddManual.OnCanExecuteChanged();
      };
    }
  }
}