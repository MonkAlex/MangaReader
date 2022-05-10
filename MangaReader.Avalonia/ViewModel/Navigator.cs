using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Services;
using LibraryViewModel = MangaReader.Avalonia.ViewModel.Explorer.LibraryViewModel;

namespace MangaReader.Avalonia.ViewModel
{
  public class Navigator : INavigator
  {
    private ObservableCollection<ExplorerTabViewModel> tabs;
    private ObservableCollection<ExplorerTabViewModel> bottomTabs;

    // public for binding only
    public ReadOnlyObservableCollection<ExplorerTabViewModel> Tabs { get; }

    // public for binding only
    public ReadOnlyObservableCollection<ExplorerTabViewModel> BottomTabs { get; }

    private bool selectedTabChanging = false;

    public ExplorerTabViewModel CurrentTab { get; private set; }

    private Task ChangeSelection<T>() where T : ExplorerTabViewModel
    {
      return ChangeSelection(Tabs.OfType<T>().FirstOrDefault() ?? BottomTabs.OfType<T>().FirstOrDefault());
    }
    
    private async Task ChangeSelection(ExplorerTabViewModel value)
    {
      if (Equals(CurrentTab, value) || Equals(value, null))
        return;

      if (selectedTabChanging)
      {
        Log.Add($"Try to change tab selection to {value}, but its already in changing.");
        return;
      }

      selectedTabChanging = true;

      try
      {
        var previous = CurrentTab;
        if (previous != null)
        {
          await previous.OnUnselected(value).ConfigureAwait(true);
        }
        CurrentTab = value;
        SelectionChanged?.Invoke(value);
        await value.OnSelected(previous).ConfigureAwait(true);
      }
      finally
      {
        selectedTabChanging = false;
      }
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
        tabs.Add(tabViewModel);
    }
    
    public void AddBottom(ExplorerTabViewModel tabViewModel)
    {
      if (tabViewModel is null)
        return;
      
      if (!Tabs.Contains(tabViewModel) && !BottomTabs.Contains(tabViewModel)) 
        bottomTabs.Add(tabViewModel);
    }

    public void Remove(ExplorerTabViewModel tabViewModel)
    {
      if (tabViewModel is null)
        return;
      
      tabs.Remove(tabViewModel);
      bottomTabs.Remove(tabViewModel);
    }

    public Navigator()
    {
      this.tabs = new ObservableCollection<ExplorerTabViewModel>();
      this.Tabs = new ReadOnlyObservableCollection<ExplorerTabViewModel>(tabs);
      this.bottomTabs = new ObservableCollection<ExplorerTabViewModel>();
      this.BottomTabs = new ReadOnlyObservableCollection<ExplorerTabViewModel>(bottomTabs);
    }
  }

  public interface INavigator
  {
    ExplorerTabViewModel CurrentTab { get; }

    ReadOnlyObservableCollection<ExplorerTabViewModel> Tabs { get; }

    ReadOnlyObservableCollection<ExplorerTabViewModel> BottomTabs { get; }

    Task Open(ExplorerTabViewModel tabViewModel);

    Task OpenLibrary();

    Task OpenSearch();

    Task OpenChangelog();

    Task ResetLibrary();

    bool Has<T>() where T : ExplorerTabViewModel;

    void Add(ExplorerTabViewModel tabViewModel);
    void AddBottom(ExplorerTabViewModel tabViewModel);

    void Remove(ExplorerTabViewModel tabViewModel);

    event Action<ExplorerTabViewModel> SelectionChanged;
  }
}
