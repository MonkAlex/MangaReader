using System.Threading.Tasks;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class ShowUpdateHistoryCommand : BaseCommand
  {
    public override Task Execute(object parameter)
    {
      new VersionHistoryModel().Show();

      return Task.CompletedTask;
    }

    public ShowUpdateHistoryCommand()
    {
      this.Name = Strings.Update_Title;
      this.Icon = "pack://application:,,,/Icons/Main/show_history.png";
    }
  }
}