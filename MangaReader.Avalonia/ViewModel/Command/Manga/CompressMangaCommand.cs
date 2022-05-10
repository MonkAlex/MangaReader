using System.Collections.Generic;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.Properties;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class CompressMangaCommand : MultipleMangasBaseCommand
  {
    public override Task Execute(IEnumerable<IManga> mangas)
    {
      foreach (var m in mangas)
        m.Compress();

      return Task.CompletedTask;
    }

    public CompressMangaCommand(SelectionModel mangaModels, LibraryViewModel library) : base(mangaModels, library)
    {
      this.Name = Strings.Manga_Action_Compress;
      this.Icon = "pack://application:,,,/Icons/Manga/compress.png";
    }
  }
}
