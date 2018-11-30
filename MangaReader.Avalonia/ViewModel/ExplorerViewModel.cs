using System.Collections.ObjectModel;
using System.Linq;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;

namespace MangaReader.Avalonia.ViewModel
{
  public class ExplorerViewModel : ViewModelBase
  {
    private ExplorerTabViewModel selectedTab;
    public ObservableCollection<ExplorerTabViewModel> Tabs { get; }

    public static ExplorerViewModel Instance { get; } = new ExplorerViewModel();

    public bool Loaded
    {
      get => loaded;
      set => RaiseAndSetIfChanged(ref loaded, value);
    }

    private bool loaded;

    public ExplorerTabViewModel SelectedTab
    {
      get { return selectedTab; }
      set
      {
        if (Equals(selectedTab, value))
          return;

        var previous = selectedTab;
        selectedTab?.OnUnselected(value);
        RaiseAndSetIfChanged(ref selectedTab, value);
        this.Loaded = selectedTab != null;
        selectedTab?.OnSelected(previous);
      }
    }

    public void SelectDefaultTab()
    {
      var hasManga = false;
      using (var context = Repository.GetEntityContext())
      {
        hasManga = context.Get<IManga>().Any();
      }

      this.SelectedTab = hasManga ? Tabs.OrderBy(t => t.Priority).FirstOrDefault() : Tabs.OfType<SearchViewModel>().FirstOrDefault();
    }

    private ExplorerViewModel()
    {
      this.Tabs = new ObservableCollection<ExplorerTabViewModel>
      {
        new LibraryViewModel(),
        new SearchViewModel(),
        new SettingsViewModel()
      };
    }
  }
}