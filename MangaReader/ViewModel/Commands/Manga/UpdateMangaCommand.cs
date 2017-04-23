using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class UpdateMangaCommand : MangaBaseCommand
  {
    public override void Execute(IManga parameter)
    {
      Library.ThreadAction(() => Library.Update(parameter));
    }

    public UpdateMangaCommand(LibraryViewModel model) : base(model)
    {
      this.Name = Strings.Manga_Action_Update;
      this.Icon = "pack://application:,,,/Icons/Manga/start_update.png";
    }
  }
}