using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class ShowSettingCommand : LibraryBaseCommand
  {
    public override void Execute(object parameter)
    {
      new SettingsForm { Owner = WindowHelper.Owner }.ShowDialog();
    }

    public ShowSettingCommand()
    {
      this.Name = Strings.Library_Action_Settings;
    }
  }
}