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
    /// <summary>
    /// Показать историю, центрировав окно по экрану.
    /// </summary>
    public VersionHistory()
    {
      InitializeComponent();
      var assembly = Assembly.GetExecutingAssembly();
      using (var reader = new StreamReader(assembly.GetManifestResourceStream("MangaReader.Update.VersionHistory.txt")))
        this.TextBox.Text = reader.ReadToEnd();
      var version = assembly.GetName().Version;
      this.Label.Text = string.Format(Strings.Update_Label_Version, version.ToString(3));
    }

    /// <summary>
    /// Показать историю, центрировав окно по родительскому.
    /// </summary>
    /// <param name="owner">Родительское окно истории изменений.</param>
    public VersionHistory(Window owner) : this()
    {
      this.Owner = owner;
      if (this.Owner != null)
        this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }
  }
}
