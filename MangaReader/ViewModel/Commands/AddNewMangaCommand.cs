using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class AddNewMangaCommand : LibraryBaseCommand
  {
    public override void Execute(object parameter)
    {
      var vm = new AddNewModel();
      vm.Show();
    }

    public AddNewMangaCommand()
    {
      this.Name = Strings.Library_Action_Add;
    }
  }
}