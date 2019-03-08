using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Avalonia.Properties;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class UpdateMangaCommand : MultipleMangasBaseCommand
  {
    public override async void Execute(IEnumerable<IManga> mangas)
    {
      await Library.ThreadAction(Library.Update(mangas.Select(m => m.Id).ToList())).LogException().ConfigureAwait(false);
    }

    public UpdateMangaCommand(Explorer.LibraryViewModel model) : base(model)
    {
      this.NeedRefresh = true;
      this.Name = Strings.Manga_Action_Update;
      this.Icon = "pack://application:,,,/Icons/Manga/start_update.png";
    }
  }
}