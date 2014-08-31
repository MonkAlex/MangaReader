using System.Windows;
using System.Windows.Controls;
using Ookii.Dialogs.Wpf;

namespace MangaReader
{
  /// <summary>
  /// Логика взаимодействия для DownloadFolderSetting.xaml
  /// </summary>
  public partial class DownloadFolderSetting : UserControl
  {
    public DownloadFolderSetting()
    {
      InitializeComponent();
    }

    private void DownloadFolderSetting_OnLoaded(object sender, RoutedEventArgs e)
    {
      this.Name.Text = this.DataContext.GetType().Name + ": ";
    }

    private void ChangeFolder_OnClick(object sender, RoutedEventArgs e)
    {
      var dialog = new VistaFolderBrowserDialog();
      if (dialog.ShowDialog() == true)
      {
        Settings.DownloadFolder = dialog.SelectedPath + "\\";
        this.FolderPath.Text = Settings.DownloadFolder;
      }
    }
  }
}
