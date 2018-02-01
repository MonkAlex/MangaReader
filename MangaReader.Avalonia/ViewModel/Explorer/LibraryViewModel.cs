using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
    private ObservableCollection<MangaModel> items;
    private string search;
    private IReactiveDerivedList<MangaModel> filteredItems;

    public ObservableCollection<MangaModel> Items
    {
      get
      {
        if (items == null)
          RefreshItems().LogException();
        return items;
      }
      set { RaiseAndSetIfChanged(ref items, value); }
    }

    public IReactiveDerivedList<MangaModel> FilteredItems
    {
      get
      {
        if (filteredItems == null)
          RefreshItems().LogException();
        return filteredItems;
      }
      private set { RaiseAndSetIfChanged(ref filteredItems, value); }
    }

    public ObservableCollection<MangaModel> SelectedMangaModels { get; }

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
      using (var context = Core.NHibernate.Repository.GetEntityContext())
      {
        var mangas = context.Get<IManga>().Select(m => new MangaModel(m)).ToList();
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
          Items = new ObservableCollection<MangaModel>(mangas);
          FilteredItems = Items.CreateDerivedCollection(
            x => x,
            Filter,
            (original, filtered) => string.Compare(original.Name, filtered.Name, StringComparison.InvariantCultureIgnoreCase));
        }, DispatcherPriority.ApplicationIdle);
      }
    }

    private bool Filter(MangaModel manga)
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
      this.SelectedMangaModels = new ObservableCollection<MangaModel>();
      this.SelectedMangaModels.CollectionChanged += SelectedMangaModelsOnCollectionChanged;
      this.Library = new Core.Services.LibraryViewModel();
      this.Library.LibraryChanged += LibraryOnLibraryChanged;
      this.Commands.Add(new UpdateWithPauseCommand(this, Library));
      this.Commands.Add(new OpenFolderCommand(this));
    }

    private void SelectedMangaModelsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
    {
#warning Может и н нужно, комманды вроде и сами достаточно умны.
      foreach (var command in Commands)
        command.OnCanExecuteChanged();
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
                this.Items.Add(new MangaModel(args.Manga));
                break;
              case MangaOperation.Deleted:
                {
                  var mangaModels = this.Items.Where(i => i.Id == args.Manga.Id).ToList();
                  foreach (var mangaModel in mangaModels)
                    this.Items.Remove(mangaModel);
                  break;
                }
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