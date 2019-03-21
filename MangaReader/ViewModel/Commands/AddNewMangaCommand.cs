using System.Threading.Tasks;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class AddNewMangaCommand : LibraryBaseCommand
  {
    public override Task Execute(object parameter)
    {
      var vm = new AddNewModel();
      vm.Show();
      return Task.CompletedTask;
    }

    public override bool CanExecute(object parameter)
    {
      return true;
    }

    public AddNewMangaCommand(LibraryViewModel model) : base(model)
    {
      this.Name = Strings.Library_Action_Add;
      this.Icon = "pack://application:,,,/Icons/Main/add_manga.png";
    }
  }
}