using System.Windows.Data;
using MangaReader.Manga;
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

    public ShowPropertiesMangaCommand(ListCollectionView view) : base(view)
    {
      this.Name = Strings.Manga_Settings;
    }
  }
}