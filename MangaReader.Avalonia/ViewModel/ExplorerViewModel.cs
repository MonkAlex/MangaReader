using System.Collections.ObjectModel;
using System.Linq;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Convertation;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;

namespace MangaReader.Avalonia.ViewModel
{
  public class ExplorerViewModel : ViewModelBase
  {
    private ExplorerTabViewModel selectedTab;
    public ObservableCollection<ExplorerTabViewModel> Tabs { get; }

    public IProcess LoadingProcess { get; set; }

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
      using (var context = Repository.GetEntityContext("Check has any manga to select default tab"))
      {
        hasManga = context.Get<IManga>().Any();
      }

      this.SelectedTab = hasManga ? Tabs.OrderBy(t => t.Priority).FirstOrDefault() : Tabs.OfType<SearchViewModel>().FirstOrDefault();
    }


    private void LoadingProcessOnStateChanged(object sender, ConvertState e)
    {
      if (e == ConvertState.Completed)
      {
        SelectDefaultTab();
      }
    }

    private ExplorerViewModel()
    {
      this.Tabs = new ObservableCollection<ExplorerTabViewModel>
      {
        new LibraryViewModel(),
        new SearchViewModel(),
        new SettingsViewModel()
      };
      LoadingProcess = new ProcessModel();
      LoadingProcess.StateChanged += LoadingProcessOnStateChanged;
    }
  }
}