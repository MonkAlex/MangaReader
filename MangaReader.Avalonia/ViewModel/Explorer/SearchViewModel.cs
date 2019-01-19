using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MangaReader.Avalonia.ViewModel.Command;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class SearchViewModel : ExplorerTabViewModel
  {
    private ObservableCollection<MangaSearchViewModel> items;
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

    public ObservableCollection<MangaSearchViewModel> Items
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

    private async void UpdateManga()
    {
      Items.Clear();
      var uniqueUris = new ConcurrentDictionary<Uri, bool>();
      var searches = ConfigStorage.Plugins.Select(p => p.GetParser().Search(Search)).ToList();
      var tasks = searches.Select(s => Task.Run(() => s.ForEachAsync(a =>
      {
        if (uniqueUris.TryAdd(a.Uri, true))
          global::Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() => Items.Add(new MangaSearchViewModel(a)));
      })));
      await Task.WhenAll(tasks.Select(t => t.LogException()));
    }

    private void AddManga()
    {
      if (Uri.TryCreate(ManualUri, UriKind.Absolute, out Uri parsedUri))
      {
        using (Repository.GetEntityContext("Try to add manga from uri"))
        {
          var manga = Mangas.Create(parsedUri);
          var model = new MangaSearchViewModel(manga);
          model.Cover = manga.Parser.GetPreviews(manga).FirstOrDefault();
          model.PreviewFindedManga.Execute(model);
        }
      }
    }

    public SearchViewModel()
    {
      this.Name = "Search";
      this.Priority = 20;
      this.Items = new ObservableCollection<MangaSearchViewModel>();
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