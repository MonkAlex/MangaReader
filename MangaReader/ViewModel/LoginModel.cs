using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MangaReader.Core.Services;
using MangaReader.Manga;
using MangaReader.ViewModel.Commands.AddManga;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel
{
  public class LoginModel : BaseViewModel
  {
    public string UserLogin { get; set; }

    public string Password { get; set; }

    public ObservableCollection<Mangas> Bookmarks { get; private set; }

    public ObservableCollection<Mangas> SelectedBookmarks { get; private set; }

    public ICommand Login { get; private set; }

    public ICommand Logout { get; private set; }

    public LoginModel(FrameworkElement view, MangaSetting setting) : base(view)
    {
      this.Login = new LoginCommand(setting);
      this.Logout = new LogoutCommand(setting);
      this.Bookmarks = new ObservableCollection<Mangas>();
      this.SelectedBookmarks = new ObservableCollection<Mangas>();
    }
  }
}