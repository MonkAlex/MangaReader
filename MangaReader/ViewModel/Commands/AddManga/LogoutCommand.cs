using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.AddManga
{
  public class LogoutCommand : BaseCommand
  {
    private MangaSetting setting;

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && setting.Login.IsLogined;
    }

    public async override void Execute(object parameter)
    {
      base.Execute(parameter);

      await setting.Login.Logout();
    }

    public LogoutCommand(MangaSetting setting)
    {
      this.setting = setting;
      this.Name = Strings.Input_Logout;
    }
  }
}