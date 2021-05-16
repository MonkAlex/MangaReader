using System;
using System.Threading.Tasks;
using MangaReader.Core.Account;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.AddManga
{
  public class LogoutCommand : BaseCommand
  {
    private ILogin login;
    private readonly Guid mangaType;

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && login.IsLogined(mangaType);
    }

    public override async Task Execute(object parameter)
    {
      await login.Logout(mangaType).ConfigureAwait(true);
    }

    public LogoutCommand(ILogin login, Guid mangaType)
    {
      this.login = login;
      this.mangaType = mangaType;
      this.Name = Strings.Input_Logout;
    }
  }
}
