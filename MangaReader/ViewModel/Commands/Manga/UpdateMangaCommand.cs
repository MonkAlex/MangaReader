using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class UpdateMangaCommand : MangaBaseCommand
  {
    public override void Execute(Mangas parameter)
    {
      Library.ThreadAction(() => Library.Update(parameter));
    }

    public UpdateMangaCommand()
    {
      this.Name = Strings.Manga_Action_Update;
    }
  }
}