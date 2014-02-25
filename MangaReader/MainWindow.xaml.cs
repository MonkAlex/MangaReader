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
            Log.Add(System.AppDomain.CurrentDomain.BaseDirectory);
            var manga = new Manga("http://readmanga.me/lilim_kiss");
            manga.Download(manga.Name.Russian, "Том_", "Глава_");
            var single = new Manga("http://readmanga.me/art_compilation_by_chiruta");
            var link = single.GetChapter("http://readmanga.me/art_compilation_by_chiruta/vol1/1").GetImageLink(0);
            MessageBox.Show(link);
            var noChapter = new Manga("http://readmanga.me/aoi____when_hikaru_was_on_the_earth");
            var firstOrDefault = noChapter.GetAllChapters().FirstOrDefault();
            if (firstOrDefault != null)
                MessageBox.Show((firstOrDefault.GetImageLink(0)));
        }
    }
}
