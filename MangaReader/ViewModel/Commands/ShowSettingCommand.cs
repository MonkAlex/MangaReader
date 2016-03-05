using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class ShowSettingCommand : LibraryBaseCommand
  {
    public override string Name { get { return Strings.Library_Action_Settings; } }

    public override void Execute(object parameter)
    {
      new SettingsForm { Owner = WindowHelper.Owner }.ShowDialog();
    }
  }
}