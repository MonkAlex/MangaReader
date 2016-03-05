using System.Windows.Data;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class ShowPropertiesMangaCommand : MangaBaseCommand
  {
    public override void Execute(Mangas manga)
    {
      base.Execute(manga);

      new MangaForm { DataContext = manga, Owner = WindowHelper.Owner }.ShowDialog();
    }

    public ShowPropertiesMangaCommand(ListCollectionView view) : base(view)
    {
      this.Name = Strings.Manga_Settings;
    }
  }
}