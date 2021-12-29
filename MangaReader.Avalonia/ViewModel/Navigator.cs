using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Avalonia.ViewModel.Command;
using MangaReader.Avalonia.ViewModel.Command.Manga;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Exception;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using LibraryViewModel = MangaReader.Avalonia.ViewModel.Explorer.LibraryViewModel;

namespace MangaReader.Avalonia.ViewModel
{
  public class Navigator : INavigator
  {
    public ObservableCollection<ExplorerTabViewModel> Tabs { get; }

    private bool selectedTabChanging = false;

    internal ExplorerTabViewModel SelectedTab { get; private set; }
    public ObservableCollection<ExplorerTabViewModel> BottomTabs { get; }

    private Task ChangeSelection<T>() where T : ExplorerTabViewModel
    {
      return ChangeSelection(Tabs.OfType<T>().FirstOrDefault() ?? BottomTabs.OfType<T>().FirstOrDefault());
    }
    
    private async Task ChangeSelection(ExplorerTabViewModel value)
    {
      if (Equals(SelectedTab, value) || Equals(value, null))
        return;

      if (selectedTabChanging)
      {
        Log.Add($"Try to change tab selection to {value}, but its already in changing.");
        return;
      }

      selectedTabChanging = true;

      try
      {
        var previous = SelectedTab;
        if (previous != null)
        {
          await previous.OnUnselected(value).ConfigureAwait(true);
        }
        SelectedTab = value;
        SelectionChanged?.Invoke(value);
        await value.OnSelected(previous).ConfigureAwait(true);
      }
      finally
      {
        selectedTabChanging = false;
      }
    }

    public IEnumerable<T> Get<T>() where T : ExplorerTabViewModel
    {
      return Tabs.OfType<T>();
    }

    public event Action<ExplorerTabViewModel> SelectionChanged;

    public async Task Open(ExplorerTabViewModel tabViewModel)
    {
      Add(tabViewModel);
      await ChangeSelection(tabViewModel).ConfigureAwait(true);
    }
    
    public Task OpenLibrary()
    {
      return ChangeSelection<LibraryViewModel>();
    }

    public Task OpenSearch()
    {
      return ChangeSelection<SearchViewModel>();
    }

    public Task OpenChangelog()
    {
      return ChangeSelection<ChangelogViewModel>();
    }

    public Task ResetLibrary()
    {
      var library = Tabs.OfType<LibraryViewModel>().FirstOrDefault();
      return library?.RefreshItems();
    }

    public bool Has<T>() where T : ExplorerTabViewModel
    { 
      return Tabs.OfType<T>().Any() || BottomTabs.OfType<T>().Any();
    }

    public void Add(ExplorerTabViewModel tabViewModel)
    {
      if (tabViewModel is null)
        return;
      
      if (!Tabs.Contains(tabViewModel) && !BottomTabs.Contains(tabViewModel)) 
        Tabs.Add(tabViewModel);
    }
    
    public void AddBottom(ExplorerTabViewModel tabViewModel)
    {
      if (tabViewModel is null)
        return;
      
      if (!Tabs.Contains(tabViewModel) && !BottomTabs.Contains(tabViewModel)) 
        BottomTabs.Add(tabViewModel);
    }

    public void Remove(ExplorerTabViewModel tabViewModel)
    {
      if (tabViewModel is null)
        return;
      
      Tabs.Remove(tabViewModel);
      BottomTabs.Remove(tabViewModel);
    }

    public Navigator()
    {
      this.Tabs = new ObservableCollection<ExplorerTabViewModel>();
      this.BottomTabs = new ObservableCollection<ExplorerTabViewModel>();
    }
  }

  public interface INavigator
  {
    Task Open(ExplorerTabViewModel tabViewModel);

    Task OpenLibrary();

    Task OpenSearch();

    Task OpenChangelog();

    Task ResetLibrary();

    bool Has<T>() where T : ExplorerTabViewModel;

    void Add(ExplorerTabViewModel tabViewModel);
    void AddBottom(ExplorerTabViewModel tabViewModel);

    void Remove(ExplorerTabViewModel tabViewModel);

    IEnumerable<T> Get<T>() where T : ExplorerTabViewModel;

    event Action<ExplorerTabViewModel> SelectionChanged;
  }
}
