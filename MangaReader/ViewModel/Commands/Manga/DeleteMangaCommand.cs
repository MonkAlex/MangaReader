using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class DeleteMangaCommand : MangaBaseCommand
  {
    public override void Execute(Mangas parameter)
    {
      base.Execute(parameter);

      Library.Remove(parameter);
    }

    public DeleteMangaCommand()
    {
      this.Name = Strings.Manga_Action_Remove;
    }
  }
}