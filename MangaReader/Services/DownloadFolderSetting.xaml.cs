using System;
using System.IO;
using System.Reflection;
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
    private PropertyInfo DonwloadFolder;

    public DownloadFolderSetting()
    {
      InitializeComponent();
    }

    private void DownloadFolderSetting_OnLoaded(object sender, RoutedEventArgs e)
    {
      DonwloadFolder = (this.DataContext as Type).GetProperty("DownloadFolder", BindingFlags.Static | BindingFlags.NonPublic);
      this.Class.Text = (this.DataContext as Type).Name + ": ";
      this.FolderPath.Text = DonwloadFolder.GetValue(null) as string;
    }

    private void ChangeFolder_OnClick(object sender, RoutedEventArgs e)
    {
      var dialog = new VistaFolderBrowserDialog();
      if (dialog.ShowDialog() == true)
      {
        DonwloadFolder.SetValue(null, dialog.SelectedPath + Path.DirectorySeparatorChar);
        this.FolderPath.Text = DonwloadFolder.GetValue(null) as string;
      }
    }
  }
}
