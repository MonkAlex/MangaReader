using System.Linq;
using System.Threading.Tasks;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Manga;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class PreviewFindedMangaCommand : BaseCommand
  {
    public override async Task Execute(object parameter)
    {
      var model = parameter as MangaSearchViewModel;
      if (model == null)
        return;

      var manga = await Mangas.Create(model.Uri).ConfigureAwait(true);
      if (manga == null)
        return;

      await manga.Refresh().ConfigureAwait(true);
      var covers = await manga.Parser.GetPreviews(manga).ConfigureAwait(true);
      manga.Cover = covers.FirstOrDefault();

      if (await manga.IsValid().ConfigureAwait(true))
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
