using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
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
    private string libraryStatus;
    private IDownloadable lastDownload;
    private bool shutdownPc;

    public LibraryViewModel Library { get; set; }

    public ListCollectionView View { get; set; }

    public ObservableCollection<MangaViewModel> MangaViewModels { get; private set; }

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

    public bool ShutdownPC
    {
      get { return shutdownPc; }
      set
      {
        shutdownPc = value;
        OnPropertyChanged();
      }
    }

    internal virtual bool Filter(object o)
    {
      var mangaModel = o as MangaBaseModel;
      if (mangaModel == null)
        return false;

      var manga = mangaModel.Manga;

      if (LibraryFilter.Uncompleted && manga.IsCompleted)
        return false;

      if (LibraryFilter.OnlyUpdate && !manga.NeedUpdate)
        return false;

      return LibraryFilter.AllowedTypes.Any(t => Equals(t.Value as MangaSetting, manga.Setting)) &&
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
      this.MangaViewModels = new ObservableCollection<MangaViewModel>(Repository.Get<IManga>().Select(m => new MangaViewModel(m, Library)));
      Library.LibraryChanged += LibraryOnLibraryChanged;
      View = new ListCollectionView(MangaViewModels)
      {
        Filter = Filter,
        CustomSort = new MangaViewModel(null, Library)
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

      this.NavigationMenu = new ObservableCollection<ContentMenuItem>();
      this.NavigationMenu.Add(new FirstMangaCommand(View));
      this.NavigationMenu.Add(new PreviousMangaCommand(View));
      this.NavigationMenu.Add(new NextMangaCommand(View));
      this.NavigationMenu.Add(new LastMangaCommand(View));
    }

    private void LibraryOnLibraryChanged(object o, LibraryViewModelArgs args)
    {
      if (args.LibraryOperation == LibraryOperation.UpdateMangaChanged)
      {
        var model = this.MangaViewModels.SingleOrDefault(m => Equals(m.Manga, args.Manga));
        switch (args.MangaOperation)
        {
          case MangaOperation.Added:
            if (model == null)
              Client.Dispatcher.Invoke(() => this.MangaViewModels.Add(new MangaViewModel(args.Manga, Library)));
            break;
          case MangaOperation.Deleted:
            if (model != null)
              Client.Dispatcher.Invoke(() => this.MangaViewModels.Remove(model));
            break;
          case MangaOperation.UpdateStarted:
            this.LastDownload = args.Manga;
            break;
          case MangaOperation.UpdateCompleted:
            this.LastDownload = null;
            break;
          case MangaOperation.None:
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }
  }
}