using System.Threading.Tasks;
using MangaReader.Core.Account;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.AddManga
{
  public class LogoutCommand : BaseCommand
  {
    private ILogin login;

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && login.IsLogined;
    }

    public override async Task Execute(object parameter)
    {
      await login.Logout().ConfigureAwait(true);
    }

    public LogoutCommand(ILogin login)
    {
      this.login = login;
      this.Name = Strings.Input_Logout;
    }
  }
}