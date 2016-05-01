using MangaReader.Manga;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel.Manga
{
  public class MangaBaseModel : BaseViewModel
  {
    public readonly Mangas Manga;

    public MangaBaseModel(Mangas manga)
    {
      this.Manga = manga;
    }
  }
}