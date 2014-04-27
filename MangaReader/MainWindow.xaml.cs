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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Settings.Update = true;
            Settings.Language = Settings.Languages.English;
            if (File.Exists("db"))
            {
                var links = File.ReadAllLines("db");
                foreach (var manga in links.Select(link => new Manga(link)))
                {
                    var folder = @"E:\Docs\Visual Studio 2010\Projects\MangaReader\MangaReader\bin\Debug\Download\" + manga.Name;
                    manga.Download(folder, "Volume_", "Chapter_");
                    Comperssion.ComperssVolumes(folder);
                }
            }
        }
    }
}