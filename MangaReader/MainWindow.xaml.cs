using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MangaReader.Services;

namespace MangaReader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Update.Initialize();
            InitializeComponent();
            this.FormLibrary.ItemsSource = Library.DatabaseMangas;
            Library.DatabaseMangas = Cache.Get();
        }

        private void Update_click(object sender, RoutedEventArgs e)
        {
            Library.Update();
        }

        private void Load_click(object sender, RoutedEventArgs e)
        {
            Library.GetMangas();
        }

        private void Mangas_clicked(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null || listBox.SelectedItems.Count == 0)
                return;

            var manga = listBox.SelectedItem as Manga;
            if (manga == null)
                return;

            Library.Update(manga, true);
        }

        private void Add_click(object sender, RoutedEventArgs e)
        {
            //TODO: необходим ввод ссылки.
            Library.Add("");
        }
    }
}