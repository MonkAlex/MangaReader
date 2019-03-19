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
using MangaReader.Core.Services.Config;
using ReactiveUI.Legacy;

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

    public double? UpdatePercent
    {
      get => updatePercent;
      set => RaiseAndSetIfChanged(ref updatePercent, value);
    }

    private double? updatePercent;

    public Core.Services.LibraryViewModel Library { get; }

    public async Task RefreshItems()
    {
      while (!Core.NHibernate.Mapping.Initialized)
      {
        Log.Add("Wait nhibernate initialization...");
        await Task.Delay(500).ConfigureAwait(true);
      }
      using (var context = Core.NHibernate.Repository.GetEntityContext("Library items loading"))
      {
        var mangas = context.Get<IManga>().Select(m => new MangaModel(m)).ToList();
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
          Items = new ObservableCollection<MangaModel>(mangas);
          FilteredItems = Items.CreateDerivedCollection(
            x => x,
            Filter,
            ConfigStorage.Instance.ViewConfig.LibraryFilter.Compare);
        }, DispatcherPriority.ApplicationIdle).ConfigureAwait(true);
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
      this.Library = new Core.Services.LibraryViewModel();
      this.Library.LibraryChanged += LibraryOnLibraryChanged;
      this.Commands.Add(new UpdateWithPauseCommand(this, Library));
      this.Commands.AddRange(new BaseCommand[] {
        new OpenFolderCommand(this),
        new ChangeUpdateMangaCommand(false, this),
        new ChangeUpdateMangaCommand(true, this),
        new UpdateMangaCommand(this),
        new CompressMangaCommand(this),
        new OpenUrlMangaCommand(this),
        new HistoryClearMangaCommand(this),
        new DeleteMangaCommand(this),
        new ShowPropertiesMangaCommand(this)
        }
      );
    }

    private async void LibraryOnLibraryChanged(object sender, LibraryViewModelArgs args)
    {
      if (items == null)
      {
        await RefreshItems().ConfigureAwait(true);
      }

      void ProcessArgs(LibraryViewModelArgs libraryViewModelArgs)
      {
        switch (libraryViewModelArgs.LibraryOperation)
        {
          case LibraryOperation.UpdateStarted:
            UpdatePercent = null;
            break;
          case LibraryOperation.UpdatePercentChanged:
            UpdatePercent = libraryViewModelArgs.Percent == 0 ? null : libraryViewModelArgs.Percent;
            break;
          case LibraryOperation.UpdateMangaChanged:
          {
            switch (libraryViewModelArgs.MangaOperation)
            {
              case MangaOperation.Added:
                this.Items.Add(new MangaModel(libraryViewModelArgs.Manga));
                break;
              case MangaOperation.Deleted:
              {
                var mangaModels = this.Items.Where(i => i.Id == libraryViewModelArgs.MangaId).ToList();
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
            UpdatePercent = null;
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      if (Dispatcher.UIThread.CheckAccess())
        ProcessArgs(args);
      else
        await Dispatcher.UIThread.InvokeAsync(() => ProcessArgs(args)).ConfigureAwait(true);
    }
  }
}