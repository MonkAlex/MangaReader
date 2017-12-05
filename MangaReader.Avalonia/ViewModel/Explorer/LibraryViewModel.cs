using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using MangaReader.Avalonia.ViewModel.Command;
using MangaReader.Avalonia.ViewModel.Command.Library;
using MangaReader.Avalonia.ViewModel.Command.Manga;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using ReactiveUI;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class LibraryViewModel : ExplorerTabViewModel
  {
    private ObservableCollection<IManga> items;
    private string search;
    private IReactiveDerivedList<IManga> filteredItems;
    private IManga selectedManga;

    public ObservableCollection<IManga> Items
    {
      get
      {
        if (items == null)
          RefreshItems().LogException();
        return items;
      }
      set { RaiseAndSetIfChanged(ref items, value); }
    }

    public IReactiveDerivedList<IManga> FilteredItems
    {
      get
      {
        if (filteredItems == null)
          RefreshItems().LogException();
        return filteredItems;
      }
      private set { RaiseAndSetIfChanged(ref filteredItems, value); }
    }

    public IManga SelectedManga
    {
      get { return selectedManga; }
      set
      {
        RaiseAndSetIfChanged(ref selectedManga, value);
        foreach (var command in Commands)
        {
          command.OnCanExecuteChanged();
        }
      }
    }

    public string Search
    {
      get { return search; }
      set
      {
        RaiseAndSetIfChanged(ref search, value);
        FilteredItems.Reset();
      }
    }

    public ObservableCollection<BaseCommand> Commands { get; }

    public Core.Services.LibraryViewModel Library { get; }

    public async Task RefreshItems()
    {
      while (!Core.NHibernate.Mapping.Initialized)
      {
        Log.Add("Wait nhibernate initialization...");
        await Task.Delay(500);
      }
#warning Тут по идее тоже DTO нужны.
      List<IManga> mangas;
      using (var context = Core.NHibernate.Repository.GetEntityContext())
      {
        mangas = context.Get<IManga>().ToList();
      }
      Dispatcher.UIThread.InvokeAsync(() =>
      {
        Items = new ObservableCollection<IManga>(mangas);
        FilteredItems = Items.CreateDerivedCollection(
          x => x,
          Filter,
          (original, filtered) => string.Compare(original.Name, filtered.Name, StringComparison.InvariantCultureIgnoreCase));
      }, DispatcherPriority.ApplicationIdle);
    }

    private bool Filter(IManga manga)
    {
      if (manga == null)
        return false;

      if (Search == null)
        return true;

      return manga.Name.IndexOf(Search, StringComparison.InvariantCultureIgnoreCase) >= 0;
    }

    public LibraryViewModel()
    {
      this.Name = "Main";
      this.Priority = 10;
      this.Commands = new ObservableCollection<BaseCommand>();
      this.Library = new Core.Services.LibraryViewModel();
      this.Library.LibraryChanged += LibraryOnLibraryChanged;
      this.Commands.Add(new UpdateWithPauseCommand(this, Library));
      this.Commands.Add(new OpenFolderCommand());
    }

    private void LibraryOnLibraryChanged(object sender, LibraryViewModelArgs args)
    {
      switch (args.LibraryOperation)
      {
        case LibraryOperation.UpdateStarted:
          break;
        case LibraryOperation.UpdatePercentChanged:
          break;
        case LibraryOperation.UpdateMangaChanged:
          {
            switch (args.MangaOperation)
            {
              case MangaOperation.Added:
                this.Items.Add(args.Manga);
                break;
              case MangaOperation.Deleted:
                this.Items.Remove(args.Manga);
                break;
              case MangaOperation.UpdateStarted:
                break;
              case MangaOperation.UpdateCompleted:
                break;
              case MangaOperation.None:
                break;
              default:
                throw new ArgumentOutOfRangeException();
            }
            break;
          }
        case LibraryOperation.UpdateCompleted:
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}