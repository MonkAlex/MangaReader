using System;
using System.Linq;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.AddManga
{
  public class AddSelected : BaseCommand
  {
    private AddNewModel model;

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      Uri uri;
      if (Uri.TryCreate(model.InputText, UriKind.Absolute, out uri))
        Library.Add(uri);

      var selectedItems = model.BookmarksModels.SelectMany(l => l.Bookmarks.Where(b => b.IsSelected));
      foreach (var manga in selectedItems)
      {
        Library.Add(manga.Value.Uri);
      }
      model.Close();
    }

    public AddSelected(AddNewModel model)
    {
      this.model = model;
      this.Name = Strings.Library_Action_Add;
    }
  }
}