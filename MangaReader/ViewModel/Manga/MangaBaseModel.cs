using System.Collections.ObjectModel;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.ViewModel.Commands.Manga;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel.Manga
{
  public class MangaBaseModel : BaseViewModel
  {
    public readonly IManga Manga;

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


    public MangaBaseModel(IManga manga, LibraryViewModel model)
    {
      this.Manga = manga;
      this.MangaMenu = new ObservableCollection<ContentMenuItem>
      {
        new OpenFolderCommand(model),
        new ChangeUpdateMangaCommand(manga != null ? manga.NeedUpdate : false, model),
        new UpdateMangaCommand(model),
        new CompressMangaCommand(model),
        new OpenUrlMangaCommand(model),
        new HistoryClearMangaCommand(model),
        new DeleteMangaCommand(model),
        new ShowPropertiesMangaCommand(model)
      };
      this.MangaMenu.First().IsDefault = true;
    }
  }
}