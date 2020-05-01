using System;
using System.Threading.Tasks;
using MangaReader.Core.Account;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.AddManga
{
  public class LoginCommand : BaseCommand
  {
    private ILogin login;
    private readonly Guid mangaType;

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && (!login.IsLogined || login.CanLogin);
    }

    public override async Task Execute(object parameter)
    {
      await login.DoLogin(mangaType).ConfigureAwait(true);
    }

    public LoginCommand(ILogin login, Guid mangaType)
    {
      this.login = login;
      this.mangaType = mangaType;
      this.Name = Strings.Input_Login;
    }
  }
}
