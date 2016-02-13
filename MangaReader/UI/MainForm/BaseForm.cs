using System.Linq;
using System.Windows.Data;
using MangaReader.Manga;
using MangaReader.Services;
using MangaReader.Services.Config;

namespace MangaReader.UI.MainForm
{
  /// <summary>
  /// Interaction logic for BaseForm.xaml
  /// </summary>
  public partial class BaseForm : System.Windows.Controls.Page
  {
    public ListCollectionView View { get; set; }

    public LibraryFilter LibraryFilter { get; set; }

    public BaseForm()
    {
      this.DataContext = this;
      Command.AddMainMenuCommands(this);
      Command.AddMangaCommands(this);
      this.Initialized += (sender, args) => LibraryWPF.Initialize(this);

      LibraryFilter = ConfigStorage.Instance.ViewConfig.LibraryFilter;

      View = new ListCollectionView(Library.LibraryMangas)
      {
        Filter = Filter,
        CustomSort = new MangasComparer()
      };

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
  }
}
