using System.IO;
using System.Windows;
using System.Windows.Controls;
using Ookii.Dialogs.Wpf;

namespace MangaReader.Services
{
  /// <summary>
  /// Логика взаимодействия для DownloadFolderSetting.xaml
  /// </summary>
  public partial class DownloadFolderSetting : UserControl
  {
    private MangaSetting Setting;

    public DownloadFolderSetting()
    {
      InitializeComponent();
    }

    private void ChangeFolder_OnClick(object sender, RoutedEventArgs e)
    {
      var dialog = new VistaFolderBrowserDialog();
      if (dialog.ShowDialog() == true)
      {
        Setting.Folder = dialog.SelectedPath + Path.DirectorySeparatorChar;
        this.FolderPath.Text = Setting.Folder;
      }
    }

    private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      Setting = e.NewValue as MangaSetting;
      if (Setting != null)
        this.FolderPath.Text = Setting.Folder;
    }
  }
}
