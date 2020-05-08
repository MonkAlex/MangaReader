using System;
using System.Threading.Tasks;
using MangaReader.Core.Account;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.AddManga
{
  public class LogInOutCommand : BaseCommand
  {
    private readonly Guid mangaType;
    private LoginCommand Login;
    private LogoutCommand Logout;
    private BaseCommand activeCommand;

    private BaseCommand ActiveCommand
    {
      get { return activeCommand; }
      set
      {
        activeCommand = value;
        OnCanExecuteChanged();
        this.Name = activeCommand.Name;
      }
    }

    public override bool CanExecute(object parameter)
    {
      return ActiveCommand.CanExecute(parameter);
    }

    public override async Task Execute(object parameter)
    {
      await ActiveCommand.Execute(parameter).ConfigureAwait(true);
    }

    public LogInOutCommand(ILogin login, Guid mangaType)
    {
      this.mangaType = mangaType;
      this.Login = new LoginCommand(login, mangaType);
      this.Logout = new LogoutCommand(login, mangaType);
      if (login != null)
      {
        LoginOnLoginStateChanged(login, login.IsLogined(mangaType));
        login.LoginStateChanged += LoginOnLoginStateChanged;
      }
    }

    private void LoginOnLoginStateChanged(object sender, bool b)
    {
      var login = (ILogin)sender;
      if (login.IsLogined(mangaType))
        this.ActiveCommand = this.Logout;
      else
        this.ActiveCommand = this.Login;
    }
  }
}
