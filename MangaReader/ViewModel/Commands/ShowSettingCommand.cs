using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Setting;

namespace MangaReader.ViewModel.Commands
{
  public class ShowSettingCommand : LibraryBaseCommand
  {
    public override void Execute(object parameter)
    {
      base.Execute(parameter);
      new SettingModel().Show();
    }

    public ShowSettingCommand(LibraryViewModel model) : base(model)
    {
      this.Name = Strings.Library_Action_Settings;
      this.Icon = "pack://application:,,,/Icons/Main/settings.png";
    }
  }
}