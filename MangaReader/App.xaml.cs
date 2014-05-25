using System;
using System.Windows;

namespace MangaReader
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnExit(object sender, ExitEventArgs e)
        {
            Cache.Save();
            Environment.Exit(0);
        }
    }
}
