using MangaReader.Core.Services;
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

    public LogInOutCommand(MangaSetting setting)
    {
      this.Login = new LoginCommand(setting);
      this.Logout = new LogoutCommand(setting);
      LoginOnLoginStateChanged(this, setting.Login.IsLogined);
      setting.Login.LoginStateChanged += LoginOnLoginStateChanged;
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