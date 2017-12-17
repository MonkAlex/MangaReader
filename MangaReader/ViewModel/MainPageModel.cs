using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using MangaReader.Core.Exception;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.Properties;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Commands;
using MangaReader.ViewModel.Commands.Manga;
using MangaReader.ViewModel.Commands.Navigation;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Manga;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel
{
  public class MainPageModel : BaseViewModel
  {
    private ICommand showSettings;
    private ICommand addNewManga;
    private ObservableCollection<ContentMenuItem> menu;
    private ICommand updateWithPause;
    private IDownloadable lastDownload;

    public LibraryViewModel Library { get; set; }

    public ListCollectionView View { get; set; }

    public ObservableCollection<MangaModel> MangaViewModels { get; private set; }

    public ObservableCollection<MangaModel> SelectedMangaModels { get; }

    public LibraryFilter LibraryFilter { get; set; }

    public IDownloadable LastDownload
    {
      get { return lastDownload; }
      set
      {
        lastDownload = value;
        OnPropertyChanged();
      }
    }

    public ICommand AddNewManga
    {
      get { return addNewManga; }
      set
      {
        addNewManga = value;
        OnPropertyChanged();
      }
    }

    public ICommand ShowSettings
    {
      get { return showSettings; }
      set
      {
        showSettings = value;
        OnPropertyChanged();
      }
    }

    public ICommand UpdateWithPause
    {
      get { return updateWithPause; }
      set
      {
        updateWithPause = value;
        OnPropertyChanged();
      }
    }

    public ObservableCollection<ContentMenuItem> Menu
    {
      get { return menu; }
      set
      {
        menu = value;
        OnPropertyChanged();
      }
    }

    public ObservableCollection<ContentMenuItem> NavigationMenu { get; set; }

    internal virtual bool Filter(object o)
    {
      var mangaModel = o as MangaModel;
      if (mangaModel == null)
        return false;

      var manga = mangaModel;

      if (LibraryFilter.Uncompleted && manga.IsCompleted)
        return false;

      if (LibraryFilter.OnlyUpdate && !manga.NeedUpdate)
        return false;

      return LibraryFilter.AllowedTypes.Any(t => Equals(t.Value, manga.SettingsId)) &&
        manga.Name.ToLowerInvariant().Contains(LibraryFilter.Name.ToLowerInvariant());
    }

    public override void Show()
    {
      base.Show();

      var skin = UI.Skin.Skins.GetSkinSetting(ConfigStorage.Instance.ViewConfig.SkinGuid);
      skin.Init();
      Log.AddFormat("Selected skin - '{0}'.", skin.Name);
      WindowModel.Instance.Content = ViewService.Instance.TryGet<System.Windows.FrameworkElement>(this);
    }

    public override void Load()
    {
      base.Load();

      this.LibraryFilter.PropertyChanged += LibraryFilterOnPropertyChanged;
    }

    public override void Unload()
    {
      base.Unload();

      this.LibraryFilter.PropertyChanged -= LibraryFilterOnPropertyChanged;
    }

    private void LibraryFilterOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
    {
      var old = View.CurrentItem;
      View.Refresh();
      if (!View.MoveCurrentTo(old))
        View.MoveCurrentToFirst();
    }

    public MainPageModel()
    {
      LibraryFilter = ConfigStorage.Instance.ViewConfig.LibraryFilter;
      this.Library = new LibraryViewModel();
      using (var context = Repository.GetEntityContext())
        this.MangaViewModels = new ObservableCollection<MangaModel>(context.Get<IManga>().Select(m => new MangaModel(m, Library)));
      this.SelectedMangaModels = new ObservableCollection<MangaModel>();
      Library.LibraryChanged += LibraryOnLibraryChanged;
      View = new ListCollectionView(MangaViewModels)
      {
        Filter = Filter,
        CustomSort = mangaComparer.Value
      };
      View.MoveCurrentToFirst();

      this.AddNewManga = new AddNewMangaCommand(Library);
      this.ShowSettings = new ShowSettingCommand(Library);
      this.UpdateWithPause = new UpdateWithPauseCommand(View, Library);

      #region Менюшка

      this.Menu = new ObservableCollection<ContentMenuItem>();

      var file = new ContentMenuItem("Файл");
      file.SubItems.Add((BaseCommand)this.AddNewManga);
      file.SubItems.Add((BaseCommand)this.UpdateWithPause);
      file.SubItems.Add(new UpdateAllCommand(Library) { Name = "Обновить всё" });
      file.SubItems.Add(new ExitCommand());

      var setting = new ContentMenuItem(Strings.Library_Action_Settings);
      setting.SubItems.Add((BaseCommand)ShowSettings);

      var about = new ContentMenuItem("О программе");
      about.SubItems.Add(new AppUpdateCommand(Library));
      about.SubItems.Add(new ShowUpdateHistoryCommand());
      about.SubItems.Add(new OpenWikiCommand());

      this.Menu.Add(file);
      this.Menu.Add(setting);
      this.Menu.Add(about);

      #endregion

      this.NavigationMenu = new ObservableCollection<ContentMenuItem>
      {
        new FirstMangaCommand(View),
        new PreviousMangaCommand(View),
        new NextMangaCommand(View),
        new LastMangaCommand(View)
      };
    }

    private Lazy<IComparer> mangaComparer = new Lazy<IComparer>(() => new MangaComparerImpl());

    private class MangaComparerImpl : IComparer, IComparer<IManga>
    {
      private static LibraryFilter setting = ConfigStorage.Instance.ViewConfig.LibraryFilter;

      public int Compare(object x, object y)
      {
        if (x is MangaModel xM && y is MangaModel yM)
        {
          if (setting.SortDescription.PropertyName == nameof(IManga.Created))
            return CompareByDate(xM.Created, yM.Created);
          if (setting.SortDescription.PropertyName == nameof(IManga.DownloadedAt))
            return CompareByDate(xM.DownloadedAt, yM.DownloadedAt);
          return CompareByName(xM.Name, yM.Name);
        }
        throw new MangaReaderException("Can compare only Mangas.");
      }

      private static int CompareByDate(DateTime? x, DateTime? y)
      {
        if (setting.SortDescription.Direction == ListSortDirection.Ascending)
          return DateTime.Compare(x ?? DateTime.MinValue, y ?? DateTime.MinValue);

        return DateTime.Compare(y ?? DateTime.MinValue, x ?? DateTime.MinValue);
      }

      private static int CompareByName(string x, string y)
      {
        if (setting.SortDescription.Direction == ListSortDirection.Ascending)
          return string.Compare(x, y, StringComparison.InvariantCultureIgnoreCase);

        return string.Compare(y, x, StringComparison.InvariantCultureIgnoreCase);
      }

      public int Compare(IManga x, IManga y)
      {
        if (setting.SortDescription.PropertyName == nameof(IManga.Created))
          return CompareByDate(x.Created, y.Created);
        if (setting.SortDescription.PropertyName == nameof(IManga.DownloadedAt))
          return CompareByDate(x.DownloadedAt, y.DownloadedAt);
        return CompareByName(x.Name, y.Name);
      }
    }

    private void LibraryOnLibraryChanged(object o, LibraryViewModelArgs args)
    {
      if (args.LibraryOperation == LibraryOperation.UpdateMangaChanged)
      {
        var model = this.MangaViewModels.SingleOrDefault(m => Equals(m.Id, args.Manga?.Id));
        switch (args.MangaOperation)
        {
          case MangaOperation.Added:
            if (model == null)
              Client.Dispatcher.Invoke(() => this.MangaViewModels.Add(new MangaModel(args.Manga, Library)));
            break;
          case MangaOperation.Deleted:
            if (model != null)
              Client.Dispatcher.Invoke(() => this.MangaViewModels.Remove(model));
            break;
          case MangaOperation.UpdateStarted:
            ActualizeSpeedAndProcess(LastDownload as IManga);
            this.LastDownload = args.Manga;
            break;
          case MangaOperation.UpdateCompleted:
            ActualizeSpeedAndProcess(args.Manga);
            this.LastDownload = null;
            break;
          case MangaOperation.None:
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
      if (args.LibraryOperation == LibraryOperation.UpdateCompleted)
      {
        if (Library.ShutdownPC)
          Client.Dispatcher.Invoke(() => new ShutdownViewModel().Show());
        Library.ShutdownPC = false;
      }
      if (args.LibraryOperation == LibraryOperation.UpdatePercentChanged)
      {
        ActualizeSpeedAndProcess(args.Manga);
      }
    }

    private void ActualizeSpeedAndProcess(IManga manga)
    {
      if (manga == null)
        return;

      var view = MangaViewModels.SingleOrDefault(m => m.Id == manga.Id);
      if (view != null)
      {
        view.Downloaded = manga.Downloaded;
        view.Speed = NetworkSpeed.TotalSpeed.HumanizeByteSize();
      }
    }
  }
}