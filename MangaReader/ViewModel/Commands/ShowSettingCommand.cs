using System.Threading.Tasks;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Setting;

namespace MangaReader.ViewModel.Commands
{
  public class ShowSettingCommand : LibraryBaseCommand
  {
    public override Task Execute(object parameter)
    {
      new SettingModel().Show();

      return Task.CompletedTask;
    }

    public ShowSettingCommand(LibraryViewModel model) : base(model)
    {
      this.Name = Strings.Library_Action_Settings;
      this.Icon = "pack://application:,,,/Icons/Main/settings.png";
    }
  }
}