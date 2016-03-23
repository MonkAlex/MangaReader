using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.UI.AddNewManga;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class AddNewMangaCommand : LibraryBaseCommand
  {
    public override void Execute(object parameter)
    {
      var vm = new AddNewModel(new AddNew() {Owner = WindowHelper.Owner});
      vm.Show();
    }

    public AddNewMangaCommand()
    {
      this.Name = Strings.Library_Action_Add;
    }
  }
}