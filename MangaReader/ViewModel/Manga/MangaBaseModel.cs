using System.Collections.ObjectModel;
using System.Linq;
using MangaReader.Manga;
using MangaReader.ViewModel.Commands.Manga;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel.Manga
{
  public class MangaBaseModel : BaseViewModel
  {
    public readonly Mangas Manga;

    private ObservableCollection<ContentMenuItem> mangaMenu;

    public ObservableCollection<ContentMenuItem> MangaMenu
    {
      get { return mangaMenu; }
      set
      {
        mangaMenu = value;
        OnPropertyChanged();
      }
    }


    public MangaBaseModel(Mangas manga)
    {
      this.Manga = manga;
      this.MangaMenu = new ObservableCollection<ContentMenuItem>
      {
        new OpenFolderCommand(),
        new ChangeUpdateMangaCommand(null),
        new UpdateMangaCommand(null),
        new CompressMangaCommand(null),
        new OpenUrlMangaCommand(null),
        new HistoryClearMangaCommand(null),
        new DeleteMangaCommand(null),
        new ShowPropertiesMangaCommand(null)
      };
      this.MangaMenu.First().IsDefault = true;
    }
  }
}