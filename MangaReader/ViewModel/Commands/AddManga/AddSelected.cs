using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.AddManga
{
  public class AddSelected : BaseCommand
  {
    private AddNewModel model;

    public override void Execute(object parameter)
    {
      base.Execute(parameter);
    }

    public AddSelected(AddNewModel model)
    {
      this.model = model;
    }
  }
}