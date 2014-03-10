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
            var manga = new Manga("http://readmanga.me/toradora");
            //manga.Download(manga.Name, "Том_", "Глава_");
            Comperssion.ComperssChapters(@"E:\Docs\Visual Studio 2010\Projects\MangaReader\MangaReader\bin\Debug\ToraDora");

          /*var manga2 = new Manga("http://readmanga.me/saki");
            manga2.Download(manga2.Name, "Том_", "Глава_");
            Comperssion.ComperssVolumes(manga2.Name);*/
            
            MessageBox.Show("ok");
        }
    }
}
