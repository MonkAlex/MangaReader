using System.Linq;
using System.Windows;
using MangaReader.Mangas;

namespace MangaReader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var manga = new Manga("http://readmanga.me/monster");
          //manga.Download(manga.Name.Russian, "Том_", "Глава_");
        }
    }
}
