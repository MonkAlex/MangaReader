using System;
using System.Collections.Generic;
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
    public ITrayIcon TrayIcon;

    private INavigator navigator;

    public ReadOnlyObservableCollection<ExplorerTabViewModel> Tabs => navigator.Tabs;

    public ReadOnlyObservableCollection<ExplorerTabViewModel> BottomTabs => navigator.BottomTabs;

    public IProcess LoadingProcess { get; set; }

    public bool Loaded
    {
      get => loaded;
      set => RaiseAndSetIfChanged(ref loaded, value);
    }

    private bool loaded;

    private bool appUpdated = false;

    internal ExplorerTabViewModel SelectedTab
    {
      get { return navigator.CurrentTab; }
      set
      {
        navigator.Open(value).LogException();
        RaisePropertyChanged();
        this.Loaded = navigator.CurrentTab != null;
      }
    }

    private async Task SelectDefaultTab()
    {
      if (appUpdated)
      {
        await navigator.OpenChangelog().ConfigureAwait(true);
        return;
      }

      var hasManga = false;
      using (var context = Repository.GetEntityContext("Check has any manga to select default tab"))
      {
        hasManga = await context.Get<IManga>().AnyAsync().ConfigureAwait(true);
      }

      if (hasManga)
      {
        await navigator.OpenLibrary().ConfigureAwait(true);
      }
      else
      {
        await navigator.OpenSearch().ConfigureAwait(true);
      }
    }


    private void LoadingProcessOnStateChanged(object sender, ConvertState e)
    {
      if (e == ConvertState.Completed)
      {
        SelectDefaultTab().LogException();
      }
    }

    internal ExplorerViewModel(INavigator navigator,
      IEnumerable<ExplorerTabViewModel> tabs,
      IEnumerable<ExplorerTabViewModel> bottomTabs,
      ITrayIcon trayIcon,
      IProcess process)
    {
      this.TrayIcon = trayIcon;
      this.navigator = navigator;
      this.navigator.SelectionChanged += OnNavigatorOnSelectionChanged;

      foreach (var tab in tabs)
      {
        navigator.Add(tab);
      }
      foreach (var tab in bottomTabs)
      {
        navigator.AddBottom(tab);
      }

      LoadingProcess = process;
      LoadingProcess.StateChanged += LoadingProcessOnStateChanged;
      Client.ClientUpdated += ClientOnClientUpdated;
    }

    private void OnNavigatorOnSelectionChanged(ExplorerTabViewModel model)
    {
      RaisePropertyChanged(nameof(SelectedTab));
      this.Loaded = navigator.CurrentTab != null;
    }

    private void ClientOnClientUpdated(object sender, Version e)
    {
      appUpdated = true;
    }

    public void Dispose()
    {
      this.navigator.SelectionChanged -= OnNavigatorOnSelectionChanged;
      Client.ClientUpdated -= ClientOnClientUpdated;
      TrayIcon?.Dispose();
    }
  }
}
