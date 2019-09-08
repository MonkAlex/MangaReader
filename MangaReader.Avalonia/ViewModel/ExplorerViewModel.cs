using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Avalonia.Platform;
using MangaReader.Avalonia.Platform.Win;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Convertation;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using LibraryViewModel = MangaReader.Avalonia.ViewModel.Explorer.LibraryViewModel;

namespace MangaReader.Avalonia.ViewModel
{
  public class ExplorerViewModel : ViewModelBase, System.IDisposable
  {
    private ExplorerTabViewModel selectedTab;

    public ITrayIcon TrayIcon;

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

#warning Тут два асинхронных обработчика, похоже нужна команда, а не свойство?
        var previous = selectedTab;
        selectedTab?.OnUnselected(value);
        RaiseAndSetIfChanged(ref selectedTab, value);
        this.Loaded = selectedTab != null;
        selectedTab?.OnSelected(previous);
      }
    }

    public async Task SelectDefaultTab()
    {
      var hasManga = false;
      using (var context = Repository.GetEntityContext("Check has any manga to select default tab"))
      {
        hasManga = await context.Get<IManga>().AnyAsync().ConfigureAwait(true);
      }

      this.SelectedTab = hasManga ? Tabs.OrderBy(t => t.Priority).FirstOrDefault() : Tabs.OfType<SearchViewModel>().FirstOrDefault();
    }


    private void LoadingProcessOnStateChanged(object sender, ConvertState e)
    {
      if (e == ConvertState.Completed)
      {
        SelectDefaultTab().LogException();
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
      TrayIcon = new WindowsTrayIcon();
      TrayIcon.SetIcon();
    }

    public void Dispose()
    {
      TrayIcon?.Dispose();
    }
  }
}
