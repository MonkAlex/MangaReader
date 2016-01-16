using Ookii.Dialogs.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace MangaReader.Services
{
  /// <summary>
  /// Interaction logic for MangaSettings.xaml
  /// </summary>
  public partial class MangaSettings : UserControl
  {
    public MangaSettings()
    {
      InitializeComponent();
    }

    private void ChangeFolder_OnClick(object sender, RoutedEventArgs e)
    {
      var dialog = new VistaFolderBrowserDialog();
      if (dialog.ShowDialog() == true)
        this.FolderPath.Text = dialog.SelectedPath + System.IO.Path.DirectorySeparatorChar;
    }
  }
}
