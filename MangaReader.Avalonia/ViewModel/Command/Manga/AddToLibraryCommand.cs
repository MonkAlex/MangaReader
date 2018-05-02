using System;
using System.Linq;
using MangaReader.Avalonia.ViewModel.Explorer;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class AddToLibraryCommand : BaseCommand
  {
    private SearchViewModel searchModel;

    public override void Execute(object parameter)
    {
      var model = parameter as MangaViewModel;
      if (model == null)
        return;
      
      var libraries = ExplorerViewModel.Instance.Tabs.OfType<LibraryViewModel>().ToList();
      var added = libraries.Any() && libraries.All(l => l.Library.Add(model.Uri));
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