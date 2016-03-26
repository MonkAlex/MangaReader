using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
    private MangaSetting setting;
    private bool isEnabled;

    public FrameworkElement View { get { return view; } }

    public string Header { get; set; }

    public string Login
    {
      get { return setting.Login.Name; }
      set
      {
        setting.Login.Name = value;
        OnPropertyChanged();
      }
    }

    public string Password
    {
      get { return setting.Login.Password; }
      set
      {
        setting.Login.Password = value;
        OnPropertyChanged();
      }
    }

    public bool CanEdit { get { return IsEnabled && !setting.Login.IsLogined; } }

    public bool IsEnabled
    {
      get { return isEnabled; }
      set
      {
        isEnabled = value;
        OnPropertyChanged();
      }
    }

    public ObservableCollection<SelectedItem<Mangas>> Bookmarks { get; private set; }

    public ICommand LogInOutCommand { get; private set; }

    public override async Task Load()
    {
      await base.Load();

      if (IsEnabled)
        return;

      await LoadBookmarks();

      this.IsEnabled = true;
      LoginOnLoginStateChanged(this, setting.Login.IsLogined);
    }

    private async Task LoadBookmarks()
    {
      var bookmarks = await setting.Login.GetBookmarks();

      foreach (var bookmark in bookmarks)
      {
        if (!Bookmarks.Any(b => Equals(b.Value.Uri, bookmark.Uri)))
          Bookmarks.Add(new SelectedItem<Mangas>(bookmark));
      }
    }

    public LoginModel(FrameworkElement view, MangaSetting setting) : base(view)
    {
      this.setting = setting;
      this.Header = setting.MangaName;
      this.LogInOutCommand = new LogInOutCommand(setting);
      this.Bookmarks = new ObservableCollection<SelectedItem<Mangas>>();
      this.setting.Login.LoginStateChanged += LoginOnLoginStateChanged;
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