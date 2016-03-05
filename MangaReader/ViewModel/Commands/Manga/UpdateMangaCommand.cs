using System.Windows.Data;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.Services;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class UpdateMangaCommand : BaseMangaCommand
  {
    public override void Execute(Mangas parameter)
    {
      Library.ThreadAction(() => Library.Update(parameter));
    }

    public UpdateMangaCommand(ListCollectionView view) : base(view)
    {
      this.Name = Strings.Manga_Action_Update;
    }
  }
}