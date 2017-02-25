using System;
using System.Linq;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Setting;

namespace MangaReader.ViewModel.Commands.AddManga
{
  public class AddSelected : BaseCommand
  {
    private AddNewModel mainModel;

    private AddFromUri model;

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      Uri uri;
      if (Uri.TryCreate(model.InputText, UriKind.Absolute, out uri))
        Library.Add(uri);

      var selectedItems = mainModel.BookmarksModels.OfType<AddBookmarksModel>()
        .SelectMany(m => m.Bookmarks.Where(b => b.IsSelected));
      foreach (var manga in selectedItems)
      {
        Library.Add(manga.Value.Uri);
      }
      mainModel.Close();
    }

    public AddSelected(AddFromUri model, AddNewModel mainModel)
    {
      this.model = model;
      this.mainModel = mainModel;
      this.Name = Strings.Library_Action_Add;
    }
  }
}