using System.Collections.ObjectModel;
using System.Linq;
using MangaReader.Avalonia.ViewModel.Explorer;

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
      this.SelectedTab = this.Tabs.OrderBy(t => t.Priority).FirstOrDefault();
    }

    private ExplorerViewModel()
    {
      this.Tabs = new ObservableCollection<ExplorerTabViewModel>
      {
        new LibraryViewModel(),
        new SearchViewModel()
      };
    }
  }
}