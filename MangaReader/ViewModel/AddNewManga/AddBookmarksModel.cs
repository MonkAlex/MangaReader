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
using MangaReader.Core.Services;

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
          LoadBookmarks().LogException();
        return bookmarks;
      }
    }

    public bool IsBookmarksLoaded { get { return bookmarks.Any(); } }

    public ICommand Add { get; }

    public LoginModel Login { get; }

    public override Task Save()
    {
#warning What the hell with this base class? Only for header property?
      throw new NotImplementedException();
    }

    private async Task LoadBookmarks()
    {
      var siteBookmarks = await Login.GetBookmarks().ConfigureAwait(true);
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
      if (b && Login.IsEnabled && !IsBookmarksLoaded)
      {
        if (Client.Dispatcher.CheckAccess())
          await LoadBookmarks().ConfigureAwait(true);
        else
          await Client.Dispatcher.InvokeAsync(async () => await LoadBookmarks().ConfigureAwait(true));
      }
    }
  }
}