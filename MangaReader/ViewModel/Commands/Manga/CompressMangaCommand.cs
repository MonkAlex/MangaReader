using System.Collections.Generic;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class CompressMangaCommand : MultipleMangasBaseCommand
  {
    public override void Execute(IEnumerable<IManga> mangas)
    {
      foreach (var m in mangas)
        m.Compress();
    }

    public CompressMangaCommand(MainPageModel model) : base(model)
    {
      this.Name = Strings.Manga_Action_Compress;
      this.Icon = "pack://application:,,,/Icons/Manga/compress.png";
    }
  }
}