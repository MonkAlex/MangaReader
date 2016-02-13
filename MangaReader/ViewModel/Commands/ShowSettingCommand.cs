using MangaReader.Properties;
using MangaReader.Services;

namespace MangaReader.ViewModel.Commands
{
  public class ShowSettingCommand : BaseCommand
  {
    public override string Name { get { return Strings.Library_Action_Settings; } }

    public override bool CanExecute(object parameter)
    {
      return Library.IsAvaible;
    }

    public override void Execute(object parameter)
    {
      new SettingsForm { Owner = WindowHelper.Owner }.ShowDialog();
    }
  }
}