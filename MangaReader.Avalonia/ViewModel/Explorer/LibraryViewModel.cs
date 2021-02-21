using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using DynamicData;
using DynamicData.Binding;
using MangaReader.Avalonia.ViewModel.Command;
using MangaReader.Avalonia.ViewModel.Command.Library;
using MangaReader.Avalonia.ViewModel.Command.Manga;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class LibraryViewModel : ExplorerTabViewModel
  {
    private SourceCache<MangaModel, int> items;
    private string search;
    private ReadOnlyObservableCollection<MangaModel> filteredItems;

    public SourceCache<MangaModel, int> Items
    {
      get
      {
        if (items == null)
          RefreshItems().LogException();
        return items;
      }
      set { RaiseAndSetIfChanged(ref items, value); }
    }

    public IEnumerable<MangaModel> FilteredItems
    {
      get
      {
        if (filteredItems == null)
          RefreshItems().LogException();
        return filteredItems;
      }
    }

    public ObservableCollection<MangaModel> SelectedMangaModels { get; }

    public string Search
    {
      get { return search; }
      set { RaiseAndSetIfChanged(ref search, value); }
    }

    public ObservableCollection<BaseCommand> LibraryCommands { get; }

    public ObservableCollection<BaseCommand> MangaCommands { get; }

    public ICommand DefaultMangaCommand { get; }

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
        var mangas = await context.Get<IManga>().Select(m => new MangaModel(m)).ToListAsync().ConfigureAwait(true);
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
          Items = new SourceCache<MangaModel, int>(m => m.Id);
          Items.AddOrUpdate(mangas);
          var filter = this.WhenValueChanged(x => x.Search).Select(Filter);
          var cancellation = Items
            .Connect()
            .Filter(filter)
            .Sort(ConfigStorage.Instance.ViewConfig.LibraryFilter)
            .ObserveOn(SynchronizationContext.Current)
            .Bind(out filteredItems)
            .DisposeMany()
            .Subscribe();

          RaisePropertyChanged(nameof(FilteredItems));
          RaisePropertyChanged(nameof(SelectedMangaModels));

        }, DispatcherPriority.ApplicationIdle).ConfigureAwait(true);
      }
    }

    private Func<MangaModel, bool> Filter(string filterName)
    {
      if (filterName == null)
        return x => true;

      return x => x.Name.IndexOf(filterName, StringComparison.InvariantCultureIgnoreCase) >= 0;
    }

    public LibraryViewModel()
    {
      this.Name = "Main";
      this.Priority = 10;
      this.LibraryCommands = new ObservableCollection<BaseCommand>();
      this.MangaCommands = new ObservableCollection<BaseCommand>();
      this.SelectedMangaModels = new ObservableCollection<MangaModel>();
      this.Library = new Core.Services.LibraryViewModel();
      this.Library.LibraryChanged += LibraryOnLibraryChanged;
      this.LibraryCommands.Add(new UpdateWithPauseCommand(this, Library));
      this.MangaCommands.AddRange(new BaseCommand[] {
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
      this.DefaultMangaCommand = this.MangaCommands.First();
    }

    private async void LibraryOnLibraryChanged(object sender, LibraryViewModelArgs args)
    {
      // Сохраняем, пока Id не почистился при удалении.
      var mangaId = args.Manga?.Id;
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
            ActualizeSpeedAndProcess(args.Manga);
            break;
          case LibraryOperation.UpdateMangaChanged:
            {
              switch (libraryViewModelArgs.MangaOperation)
              {
                case MangaOperation.Added:
                  this.Items.AddOrUpdate(new MangaModel(libraryViewModelArgs.Manga));
                  break;
                case MangaOperation.Deleted:
                  {
                    var mangaModels = this.Items.Items.Where(i => i.Id == mangaId).ToList();
                    foreach (var mangaModel in mangaModels)
                      this.Items.Remove(mangaModel);
                    break;
                  }
                case MangaOperation.UpdateStarted:
                  ActualizeSpeedAndProcess(args.Manga);
                  break;
                case MangaOperation.UpdateCompleted:
                  ActualizeSpeedAndProcess(args.Manga);
                  ExplorerViewModel.Instance.TrayIcon.ShowBalloon($"Обновление {args.Manga.Name} завершено.", args.Manga);
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

    private void ActualizeSpeedAndProcess(IManga manga)
    {
      if (manga == null)
        return;

      var view = Items.Items.SingleOrDefault(m => m.Id == manga.Id);
      if (view != null)
      {
        view.Downloaded = manga.Downloaded;
        view.Speed = NetworkSpeed.TotalSpeed.HumanizeByteSize();
      }
    }
  }
}
