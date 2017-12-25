using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class UpdateMangaCommand : MultipleMangasBaseCommand
  {
    public override void Execute(IEnumerable<IManga> mangas)
    {
      Library.ThreadAction(() => Library.Update(mangas.Select(m => m.Id).ToList())).LogException();
    }

    public UpdateMangaCommand(MainPageModel model) : base(model)
    {
      this.NeedRefresh = true;
      this.Name = Strings.Manga_Action_Update;
      this.Icon = "pack://application:,,,/Icons/Manga/start_update.png";
    }
  }
}