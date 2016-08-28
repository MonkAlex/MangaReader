using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using MangaReader.Core.Account;
using MangaReader.Core.Manga;
using MangaReader.ViewModel.Commands.AddManga;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel
{
  public class LoginModel : BaseViewModel
  {
    private ILogin login;
    private bool isEnabled;

    public string Login
    {
      get { return login.Name; }
      set
      {
        login.Name = value;
        OnPropertyChanged();
      }
    }

    public string Password
    {
      get { return login.Password; }
      set
      {
        login.Password = value;
        OnPropertyChanged();
      }
    }

    public bool CanEdit { get { return IsEnabled && !login.IsLogined; } }

    public bool IsEnabled
    {
      get { return isEnabled; }
      set
      {
        isEnabled = value;
        OnPropertyChanged();
        OnPropertyChanged(nameof(CanEdit));
      }
    }

    public bool HasLogin { get; set; }

    public ICommand LogInOutCommand { get; private set; }

    public async Task<List<IManga>> GetBookmarks()
    {
      if (login != null)
        return await login.GetBookmarks();
      return new List<IManga>();
    }

    public LoginModel(ILogin login)
    {
      if (login != null)
      {
        this.login = login;
        this.LogInOutCommand = new LogInOutCommand(this.login);
        this.login.LoginStateChanged += LoginOnLoginStateChanged;
        HasLogin = true;
      }
      else
        HasLogin = false;
    }

    private void LoginOnLoginStateChanged(object sender, bool b)
    {
      OnPropertyChanged(nameof(CanEdit));
      OnPropertyChanged(nameof(Login));
      OnPropertyChanged(nameof(Password));
    }
  }
}