using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MangaReader.Core.Account;
using MangaReader.Core.Manga;
using MangaReader.ViewModel.Commands.AddManga;
using MangaReader.ViewModel.Primitive;
using MangaReader.ViewModel.Setting;

namespace MangaReader.ViewModel
{
  public class AddBookmarksModel : SettingViewModel
  {
    private readonly ObservableCollection<SelectedItem<IManga>> bookmarks;

    public ObservableCollection<SelectedItem<IManga>> Bookmarks
    {
      get
      {
        if (!bookmarks.Any())
          LoadBookmarks();
        return bookmarks;
      }
    }

    public ICommand Add { get; }

    public LoginModel Login { get; }
    
    private async Task LoadBookmarks()
    {
      var siteBookmarks = await Login.GetBookmarks();
      foreach (var bookmark in siteBookmarks)
      {
        if (!bookmarks.Any(b => Equals(b.Value.Uri, bookmark.Uri)))
          bookmarks.Add(new SelectedItem<IManga>(bookmark));
      }
      Login.IsEnabled = Login.HasLogin;
    }

    public AddBookmarksModel(ILogin login, string name, AddFromUri addFromUriModel, AddNewModel mainModel)
    {
      this.bookmarks = new ObservableCollection<SelectedItem<IManga>>();
      this.Header = name;
      this.Add = new AddSelected(addFromUriModel, mainModel);
      this.Login = new LoginModel(login);
      if (login != null)
        login.LoginStateChanged += LoginOnLoginStateChanged;
    }

    private async void LoginOnLoginStateChanged(object sender, bool b)
    {
      if (b && Login.IsEnabled && !Bookmarks.Any())
      {
        await LoadBookmarks();
      }
    }
  }
}