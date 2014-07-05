using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MangaReader.Logins;

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
            Result.Focus();
            this.Login.Text = Settings.Login.Name;
            this.Password.Password = Settings.Login.Password;
        }

        public string ResponseText
        {
            get { return Result.Text; }
            set { Result.Text = value; }
        }

        private void Login_click(object sender, RoutedEventArgs e)
        {
            Settings.Login = new Login() { Name = Login.Text, Password = Password.Password };
            Grouple.Login();
            this.Bookmarks.ItemsSource = Grouple.LoadBookmarks();
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

            if (!(e.MouseDevice.DirectlyOver is Image))
                listBox.SelectedIndex = -1;
        }
    }
}
