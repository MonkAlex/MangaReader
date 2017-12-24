using MangaReader.Core.Account;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.AddManga
{
  public class LoginCommand : BaseCommand
  {
    private ILogin login;

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && (!login.IsLogined || login.CanLogin);
    }

    public override async void Execute(object parameter)
    {
      base.Execute(parameter);

      await login.DoLogin();
    }

    public LoginCommand(ILogin login)
    {
      this.login = login;
      this.Name = Strings.Input_Login;
    }
  }
}