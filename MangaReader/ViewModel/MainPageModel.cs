using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using MangaReader.Manga;
using MangaReader.Services;
using MangaReader.Services.Config;
using MangaReader.ViewModel.Commands;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel
{
  public class MainPageModel : BaseViewModel
  {
    private ICommand showSettings;
    private ICommand addNewManga;
    private ICommand closeApplication;
    private ICommand checkUpdateApplication;

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

    public ICommand CheckUpdateApplication
    {
      get { return checkUpdateApplication; }
      set
      {
        checkUpdateApplication = value;
        OnPropertyChanged();
      }
    }

    public ICommand CloseApplication
    {
      get { return closeApplication; }
      set
      {
        closeApplication = value;
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

    public MainPageModel(FrameworkElement view) : base(view)
    {
      LibraryFilter = ConfigStorage.Instance.ViewConfig.LibraryFilter;

      View = new ListCollectionView(Library.LibraryMangas)
      {
        Filter = Filter,
        CustomSort = new MangasComparer()
      };

      this.AddNewManga = new AddNewMangaCommand();
      this.ShowSettings = new ShowSettingCommand();
      this.CheckUpdateApplication = new AppUpdateCommand();
      this.CloseApplication = new ExitCommand();
    }
  }
}