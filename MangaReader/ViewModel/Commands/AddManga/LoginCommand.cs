using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.AddManga
{
  public class LoginCommand : BaseCommand
  {
    private MangaSetting setting;

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && (!setting.Login.IsLogined || setting.Login.CanLogin);
    }

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      setting.Login.DoLogin();
    }

    public LoginCommand(MangaSetting setting)
    {
      this.setting = setting;
      this.Name = Strings.Input_Login;
    }
  }
}