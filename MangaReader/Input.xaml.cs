using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MangaReader.Logins;
using MangaReader.Properties;

namespace MangaReader
{
    /// <summary>
    /// Логика взаимодействия для Input.xaml
    /// </summary>
    public partial class Input : Window
    {
        public Input()
        {
            InitializeComponent();
            this.Bookmarks.ItemsSource = Grouple.Bookmarks;
            Result.Focus();
            this.Login.Text = Settings.Login.Name;
            this.Password.Password = Settings.Login.Password;
            this.Login.IsEnabled = !Grouple.IsLogined;
            this.Password.IsEnabled = !Grouple.IsLogined;
            this.Enter.Content = Grouple.IsLogined ? Strings.Input_Logout : Strings.Input_Login;
        }

        public string ResponseText
        {
            get { return Result.Text; }
            set { Result.Text = value; }
        }

        private void Login_click(object sender, RoutedEventArgs e)
        {
            var logined = Grouple.IsLogined;
            if (!logined)
            {
                Settings.Login = new Login() { Name = Login.Text, Password = Password.Password };
                Grouple.Login();
            }
            else
                Grouple.Logout();
            logined = Grouple.IsLogined;

            this.Bookmarks.ItemsSource = logined ? Grouple.Bookmarks : null;
            this.Login.IsEnabled = !logined;
            this.Password.IsEnabled = !logined;
            this.Enter.Content = logined ? Strings.Input_Logout : Strings.Input_Login;
        }

        private void Add_click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            this.Close();
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
