using MangaReader.Core.Manga;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Manga;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class ShowPropertiesMangaCommand : MangaBaseCommand
  {
    public override void Execute(Mangas manga)
    {
      base.Execute(manga);

      new MangaCardModel(manga).Show();
    }

    public ShowPropertiesMangaCommand()
    {
      this.Name = Strings.Manga_Settings;
      this.Icon = "pack://application:,,,/Icons/Manga/settings.png";
    }
  }
}