using System.Linq;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class PreviewFindedMangaCommand : BaseCommand
  {
    public override void Execute(object parameter)
    {
      var model = parameter as MangaSearchViewModel;
      if (model == null)
        return;

      var manga = Mangas.Create(model.Uri);
      manga.Cover = model.Cover;
      manga.Refresh();
      if (manga.IsValid())
      {
        var explorer = ExplorerViewModel.Instance;
        var searchTab = new MangaModel(manga);
        explorer.Tabs.Add(searchTab);
        explorer.SelectedTab = searchTab;
      }
    }

    public PreviewFindedMangaCommand()
    {
      this.Name = "Preview";
    }
  }
}