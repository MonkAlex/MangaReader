using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
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
            Settings.WorkFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Update.Initialize();
            InitializeComponent();
        }

        private void Update_click(object sender, RoutedEventArgs e)
        {
            Settings.DownloadFolder = @"E:\Docs\Visual Studio 2010\Projects\MangaReader\MangaReader\bin\Debug\Download\";
            Settings.Language = Settings.Languages.English;
            Library.Update();
        }

        private void Compess_click(object sender, RoutedEventArgs e)
        {
            Settings.Update = true;
            Settings.Language = Settings.Languages.English;
            var manga = new Manga("http://readmanga.me/nanohana_no_kare");
            var folder = @"E:\Docs\Visual Studio 2010\Projects\MangaReader\MangaReader\bin\Debug\Download\" + manga.Name;
            manga.Download(folder, "Volume_", "Chapter_");
            Comperssion.ComperssVolumes(folder);
        }
    }
}