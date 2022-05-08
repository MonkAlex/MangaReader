using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Avalonia.Properties;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class UpdateMangaCommand : MultipleMangasBaseCommand
  {
    public override async Task Execute(IEnumerable<IManga> mangas)
    {
      await Library.ThreadAction(Library.Update(mangas.Select(m => m.Id).ToList())).LogException().ConfigureAwait(true);
    }

    public UpdateMangaCommand(SelectionModel mangaModels, LibraryViewModel library) : base(mangaModels, library)
    {
      this.Name = Strings.Manga_Action_Update;
      this.Icon = "pack://application:,,,/Icons/Manga/start_update.png";
    }
  }
}
