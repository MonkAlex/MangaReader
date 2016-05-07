using System.Windows.Data;
using MangaReader.Core.Manga;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class ChangeUpdateMangaCommand : MangaBaseCommand
  {
    public override void Execute(Mangas manga)
    {
      base.Execute(manga);

      manga.NeedUpdate = !manga.NeedUpdate;
      manga.Save();
    }

    public ChangeUpdateMangaCommand(ListCollectionView view) : base(view)
    {
      this.Name = "Сменить статус обновления";
    }
  }
}