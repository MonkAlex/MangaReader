using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MangaReader.Account;
using MangaReader.Manga.Grouple;
using MangaReader.Properties;
using MangaReader.Services;

namespace MangaReader.UI
{
  /// <summary>
  /// Interaction logic for Login.xaml
  /// </summary>
  public partial class Login : UserControl
  {
    private MangaSetting ReadManga = Settings.MangaSettings.FirstOrDefault(x => x.Manga == Readmanga.Type);

    public Login()
    {
      InitializeComponent();
      this.Bookmarks.ItemsSource = Grouple.Bookmarks;
      if (this.ReadManga != null)
      {
        this.LoginBox.Text = ReadManga.Login.Name;
        this.Password.Password = ReadManga.Login.Password;
      }
      this.LoginBox.IsEnabled = !Grouple.IsLogined;
      this.Password.IsEnabled = !Grouple.IsLogined;
      this.Enter.Content = Grouple.IsLogined ? Strings.Input_Logout : Strings.Input_Login;
    }

    private void Login_click(object sender, RoutedEventArgs e)
    {
      var logined = Grouple.IsLogined;
      if (!logined)
      {
        if (ReadManga != null)
        {
          ReadManga.Login.Name = LoginBox.Text;
          ReadManga.Login.Password = Password.Password;
          ReadManga.Save();
        }
        Grouple.Login();
      }
      else
        Grouple.Logout();
      logined = Grouple.IsLogined;

      this.Bookmarks.ItemsSource = logined ? Grouple.Bookmarks : null;
      this.LoginBox.IsEnabled = !logined;
      this.Password.IsEnabled = !logined;
      this.Enter.Content = logined ? Strings.Input_Logout : Strings.Input_Login;
    }

    private void Bookmarks_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
      var listBox = sender as ListBox;
      if (listBox == null || listBox.SelectedItems.Count == 0)
        return;

      if (!(e.MouseDevice.DirectlyOver is TextBlock))
        listBox.SelectedIndex = -1;
    }
  }
}
