using System.Linq;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.NHibernate;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class AddToLibraryCommand : BaseCommand
  {
    private SearchViewModel searchModel;

    public override void Execute(object parameter)
    {
      var model = parameter as MangaSearchViewModel;
      if (model == null)
        return;
      
      var libraries = ExplorerViewModel.Instance.Tabs.OfType<LibraryViewModel>().ToList();
      var added = false;
      using (var context = Repository.GetEntityContext("Add selected manga to library"))
      {
        added = libraries.Any() && libraries.All(l =>
        {
          var result = l.Library.Add(model.Uri, out var manga);
          if (result && manga.Cover == null)
          {
            manga.Cover = model.Cover;
            context.Save(manga);
          }
          return result;
        });
      }
      if (added)
      {
        searchModel.Items.Remove(model);
        ExplorerViewModel.Instance.SelectedTab = (ExplorerTabViewModel)libraries.FirstOrDefault() ?? searchModel;
      }
    }

    public AddToLibraryCommand(SearchViewModel searchModel)
    {
      this.Name = "Add";
      this.searchModel = searchModel;
    }
  }
}