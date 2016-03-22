using System.Linq;
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

      foreach (var manga in model.Logins.SelectMany(l => l.SelectedBookmarks))
      {
        Library.Add(manga.Uri);
      }
    }

    public AddSelected(AddNewModel model)
    {
      this.model = model;
    }
  }
}