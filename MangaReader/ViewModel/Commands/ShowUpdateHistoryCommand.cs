using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class ShowUpdateHistoryCommand : BaseCommand
  {
    public override string Name { get { return Strings.Update_Title; } }

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      new VersionHistoryModel(new VersionHistoryView(WindowHelper.Owner)).Show();
    }
  }
}