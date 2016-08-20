using MangaReader.Core.Account;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.AddManga
{
  public class LogInOutCommand : BaseCommand
  {
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

    public override void Execute(object parameter)
    {
      ActiveCommand.Execute(parameter);
    }

    public LogInOutCommand(ILogin login)
    {
      this.Login = new LoginCommand(login);
      this.Logout = new LogoutCommand(login);
      if (login != null)
      {
        LoginOnLoginStateChanged(this, login.IsLogined);
        login.LoginStateChanged += LoginOnLoginStateChanged;
      }
    }

    private void LoginOnLoginStateChanged(object sender, bool b)
    {
      if (b)
        this.ActiveCommand = this.Logout;
      else
        this.ActiveCommand = this.Login;
    }
  }
}