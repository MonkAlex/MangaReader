using System.IO;
using System.Reflection;
using System.Windows;
using MangaReader.Properties;

namespace MangaReader.Services
{
    /// <summary>
    /// Логика взаимодействия для VersionHistory.xaml
    /// </summary>
    public partial class VersionHistory : Window
    {
        public VersionHistory()
        {
            InitializeComponent();
            var assembly = Assembly.GetExecutingAssembly();
            using (var reader = new StreamReader(assembly.GetManifestResourceStream("MangaReader.Update.VersionHistory.txt")))
                this.TextBox.Text = reader.ReadToEnd();
            var version = assembly.GetName().Version;
            this.Label.Text = string.Format(Strings.Update_Label_Version, version.Major, version.Minor, version.Build);
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
