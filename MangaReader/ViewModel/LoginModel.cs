using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using MangaReader.Manga;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel
{
  public class LoginModel : BaseViewModel
  {
    public string UserLogin { get; set; }

    public string Password { get; set; }

    public ObservableCollection<Mangas> Bookmarks { get; private set; }

    public ICommand Login { get; private set; }

    public ICommand Logout { get; private set; }

    public LoginModel(FrameworkElement view) : base(view)
    {
      this.Bookmarks = new ObservableCollection<Mangas>();
    }
  }
}