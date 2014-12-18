using System;
using System.IO;
using System.Linq;
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
    private Settings.SubclassDownloadFolder DonwloadFolder;

    public DownloadFolderSetting()
    {
      InitializeComponent();
    }

    private void DownloadFolderSetting_OnLoaded(object sender, RoutedEventArgs e)
    {
      DonwloadFolder = Settings.DownloadFolders.First(f => f.SubType == (this.DataContext as Type));
      this.Class.Text = DonwloadFolder.SubType.Name + ": ";
      this.FolderPath.Text = DonwloadFolder.Folder;
    }

    private void ChangeFolder_OnClick(object sender, RoutedEventArgs e)
    {
      var dialog = new VistaFolderBrowserDialog();
      if (dialog.ShowDialog() == true)
      {
        DonwloadFolder.Folder = dialog.SelectedPath + Path.DirectorySeparatorChar;
        this.FolderPath.Text = DonwloadFolder.Folder;
      }
    }
  }
}
