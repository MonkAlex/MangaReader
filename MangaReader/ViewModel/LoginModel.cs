using System.Collections.ObjectModel;
using System.Linq;
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
    private Login login;
    private bool isEnabled;

    public string Header { get; set; }

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
      }
    }

    public ObservableCollection<SelectedItem<IManga>> Bookmarks { get; }

    public ICommand LogInOutCommand { get; private set; }

    public override async void Load()
    {
      base.Load();

      if (IsEnabled)
        return;

      await LoadBookmarks();

      this.IsEnabled = true;
      LoginOnLoginStateChanged(this, login.IsLogined);
    }

    private async Task LoadBookmarks()
    {
      var bookmarks = await login.GetBookmarks();

      foreach (var bookmark in bookmarks)
      {
        if (!Bookmarks.Any(b => Equals(b.Value.Uri, bookmark.Uri)))
          Bookmarks.Add(new SelectedItem<IManga>(bookmark));
      }
    }

    public LoginModel(Login login, string name)
    {
      this.login = login;
      this.Header = name;
      this.LogInOutCommand = new LogInOutCommand(this.login);
      this.Bookmarks = new ObservableCollection<SelectedItem<IManga>>();
      this.login.LoginStateChanged += LoginOnLoginStateChanged;
    }

    private async void LoginOnLoginStateChanged(object sender, bool b)
    {
      OnPropertyChanged(nameof(CanEdit));
      OnPropertyChanged(nameof(Login));
      OnPropertyChanged(nameof(Password));
      if (b && IsEnabled && !Bookmarks.Any())
      {
        await LoadBookmarks();
      }
    }
  }
}