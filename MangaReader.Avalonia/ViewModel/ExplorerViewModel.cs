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

    public ExplorerTabViewModel SelectedTab
    {
      get { return selectedTab; }
      set { RaiseAndSetIfChanged(ref selectedTab, value); }
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