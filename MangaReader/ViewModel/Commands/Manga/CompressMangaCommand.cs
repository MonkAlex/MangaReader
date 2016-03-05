using System.Windows.Data;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class CompressMangaCommand : MangaBaseCommand
  {
    public override void Execute(Mangas manga)
    {
      base.Execute(manga);

      manga.Compress();
    }

    public CompressMangaCommand(ListCollectionView view) : base(view)
    {
      this.Name = Strings.Manga_Action_Compress;
    }
  }
}