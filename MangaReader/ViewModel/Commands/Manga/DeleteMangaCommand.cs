using System.Windows.Data;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.Services;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class DeleteMangaCommand : BaseMangaCommand
  {
    public override void Execute(Mangas parameter)
    {
      base.Execute(parameter);

      Library.Remove(parameter);
    }

    public DeleteMangaCommand(ListCollectionView view) : base(view)
    {
      this.Name = Strings.Manga_Action_Remove;
    }
  }
}