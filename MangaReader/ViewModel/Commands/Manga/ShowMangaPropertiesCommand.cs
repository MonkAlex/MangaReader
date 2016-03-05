using System.Windows.Data;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.Services;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class ShowMangaPropertiesCommand : BaseMangaCommand
  {
    public override void Execute(Mangas manga)
    {
      base.Execute(manga);

      new MangaForm { DataContext = manga, Owner = WindowHelper.Owner }.ShowDialog();
    }

    public ShowMangaPropertiesCommand(ListCollectionView view) : base(view)
    {
      this.Name = Strings.Manga_Settings;
    }
  }
}