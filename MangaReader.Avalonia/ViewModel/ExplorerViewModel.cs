using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Avalonia.Platform;
using MangaReader.Avalonia.Platform.Win;
using MangaReader.Avalonia.ViewModel.Command;
using MangaReader.Avalonia.ViewModel.Command.Manga;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core;
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

    public ObservableCollection<ExplorerTabViewModel> BottomTabs { get; }

    public IProcess LoadingProcess { get; set; }

    public static ExplorerViewModel Instance { get; } = new ExplorerViewModel();

    public bool Loaded
    {
      get => loaded;
      set => RaiseAndSetIfChanged(ref loaded, value);
    }

    private bool loaded;

    private bool selectedTabChanging = false;
    private bool appUpdated = false;

    public ExplorerTabViewModel SelectedTab
    {
      get { return selectedTab; }
      set
      {
        if (Equals(selectedTab, value) || Equals(value, null))
          return;

        if (selectedTabChanging)
        {
          Log.Add($"Try to change tab selection to {value}, but its already in changing.");
          return;
        }

        selectedTabChanging = true;

        try
        {
#warning Тут два асинхронных обработчика, похоже нужна команда, а не свойство?
          var previous = selectedTab;
          selectedTab?.OnUnselected(value);
          RaiseAndSetIfChanged(ref selectedTab, value);
          this.Loaded = selectedTab != null;
          selectedTab?.OnSelected(previous);
        }
        finally
        {
          selectedTabChanging = false;
        }
      }
    }

    public async Task SelectDefaultTab()
    {
      if (appUpdated)
      {
        this.SelectedTab = this.BottomTabs.OfType<ChangelogViewModel>().FirstOrDefault();
        return;
      }

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
        new SettingsViewModel(),
      };
      this.BottomTabs = new ObservableCollection<ExplorerTabViewModel>
      {
        new ChangelogViewModel(),
      };
      LoadingProcess = new ProcessModel();
      LoadingProcess.StateChanged += LoadingProcessOnStateChanged;
      Client.ClientUpdated += ClientOnClientUpdated;
      TrayIcon = new WindowsTrayIcon();
      TrayIcon.SetIcon();
      TrayIcon.DoubleClickCommand = new ShowMainWindowCommand();
      TrayIcon.BalloonClickedCommand = new OpenFolderCommandBase();
    }

    private void ClientOnClientUpdated(object sender, Version e)
    {
      appUpdated = true;
    }

    public void Dispose()
    {
      TrayIcon?.Dispose();
    }
  }
}
