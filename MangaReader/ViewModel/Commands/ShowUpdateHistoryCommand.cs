using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class ShowUpdateHistoryCommand : BaseCommand
  {
    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      new VersionHistoryModel().Show();
    }

    public ShowUpdateHistoryCommand()
    {
      this.Name = Strings.Update_Title;
    }
  }
}