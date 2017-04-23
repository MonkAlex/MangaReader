using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Manga;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class ShowPropertiesMangaCommand : MangaBaseCommand
  {
    public override void Execute(IManga manga)
    {
      base.Execute(manga);

      new MangaCardModel(manga, Library).Show();
    }

    public ShowPropertiesMangaCommand(LibraryViewModel model) : base(model)
    {
      this.Name = Strings.Manga_Settings;
      this.Icon = "pack://application:,,,/Icons/Manga/settings.png";
    }
  }
}