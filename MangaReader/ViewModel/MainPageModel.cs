using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands;
using MangaReader.ViewModel.Commands.Manga;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel
{
  public class MainPageModel : BaseViewModel
  {
    private ICommand showSettings;
    private ICommand addNewManga;
    private ObservableCollection<ContentMenuItem> menu;
    private ObservableCollection<ContentMenuItem> mangaMenu;
    private ICommand updateWithPause;
    private string libraryStatus;

    public string LibraryStatus
    {
      get { return libraryStatus; }
      set
      {
        libraryStatus = value;
        OnPropertyChanged();
      }
    }

    public ListCollectionView View { get; set; }

    public LibraryFilter LibraryFilter { get; set; }

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

    public ObservableCollection<ContentMenuItem> MangaMenu
    {
      get { return mangaMenu; }
      set
      {
        mangaMenu = value;
        OnPropertyChanged();
      }
    }


    internal virtual bool Filter(object o)
    {
      var manga = o as Mangas;
      if (manga == null)
        return false;

      if (LibraryFilter.Uncompleted && manga.IsCompleted)
        return false;

      if (LibraryFilter.OnlyUpdate && !manga.NeedUpdate)
        return false;

      return LibraryFilter.AllowedTypes.Any(t => (t.Value as MangaSetting).Manga == manga.GetType().TypeProperty()) &&
        manga.Name.ToLowerInvariant().Contains(LibraryFilter.Name.ToLowerInvariant());
    }

    public MainPageModel()
    {
      LibraryFilter = ConfigStorage.Instance.ViewConfig.LibraryFilter;

      View = new ListCollectionView(Library.LibraryMangas)
      {
        Filter = Filter,
        CustomSort = new MangasComparer()
      };

      this.AddNewManga = new AddNewMangaCommand();
      this.ShowSettings = new ShowSettingCommand();
      this.UpdateWithPause = new UpdateWithPauseCommand(View);

      #region Менюшка

      this.Menu = new ObservableCollection<ContentMenuItem>();

      var file = new ContentMenuItem("Файл");
      file.SubItems.Add((BaseCommand)this.AddNewManga);
      file.SubItems.Add(new UpdateVisibleMangaCommand(View));
      file.SubItems.Add(new UpdateAllCommand { Name = "Обновить всё" });
      file.SubItems.Add(new ExitCommand());

      var setting = new ContentMenuItem(Strings.Library_Action_Settings);
      setting.SubItems.Add((BaseCommand)ShowSettings);

      var about = new ContentMenuItem("О программе");
      about.SubItems.Add(new AppUpdateCommand());
      about.SubItems.Add(new ShowUpdateHistoryCommand());

      this.Menu.Add(file);
      this.Menu.Add(setting);
      this.Menu.Add(about);

      this.MangaMenu = new ObservableCollection<ContentMenuItem>
      {
        new OpenFolderCommand(),
        new ChangeUpdateMangaCommand(View),
        new UpdateMangaCommand(View),
        new CompressMangaCommand(View),
        new OpenUrlMangaCommand(View),
        new HistoryClearMangaCommand(View),
        new DeleteMangaCommand(View),
        new ShowPropertiesMangaCommand(View)
      };
      this.MangaMenu.First().IsDefault = true;

      #endregion
    }
  }
}