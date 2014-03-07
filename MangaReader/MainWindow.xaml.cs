using System.Windows;

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
            var manga = new Manga("http://readmanga.me/akb49___the_rules_against_love");
            manga.Download(manga.Name, "Том_", "Глава_");
          //Comperssion.ComperssVolumes(manga.Name);
        }
    }
}
