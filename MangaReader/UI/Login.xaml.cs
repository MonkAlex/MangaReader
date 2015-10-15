using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
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
    internal MangaSetting Setting { get; set; }

    public Login(MangaSetting setting)
    {
      InitializeComponent();
      this.Setting = setting;
    }

    private void Login_click(object sender, RoutedEventArgs e)
    {
      var logined = Grouple.IsLogined;
      if (!logined)
      {
        if (Setting != null)
        {
          Setting.Login.Name = LoginBox.Text;
          Setting.Login.Password = Password.Password;
          Setting.Save();
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

    private void Login_OnLoaded(object sender, RoutedEventArgs e)
    {
      this.IsEnabled = false;
      
      ThreadStart action = () =>
      {
        var bookmarks = Grouple.Bookmarks;
        this.Dispatcher.Invoke(() =>
        {
          this.Bookmarks.ItemsSource = bookmarks;
          if (this.Setting != null)
          {
            this.LoginBox.Text = Setting.Login.Name;
            this.Password.Password = Setting.Login.Password;
          }
          this.LoginBox.IsEnabled = !Grouple.IsLogined;
          this.Password.IsEnabled = !Grouple.IsLogined;
          this.Enter.Content = Grouple.IsLogined ? Strings.Input_Logout : Strings.Input_Login;

          this.IsEnabled = true;
        });
      };

      Thread _loadThread = null;
      if (_loadThread == null || _loadThread.ThreadState == ThreadState.Stopped)
        _loadThread = new Thread(action);
      if (_loadThread.ThreadState == ThreadState.Unstarted)
        _loadThread.Start();
    }
  }
}
