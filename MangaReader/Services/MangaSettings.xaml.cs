using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MangaReader.Services
{
  /// <summary>
  /// Interaction logic for MangaSettings.xaml
  /// </summary>
  public partial class MangaSettings : TabItem
  {
    public MangaSettings()
    {
      InitializeComponent();
    }

    private void OkButton_OnClick(object sender, RoutedEventArgs e)
    {
      var setting = this.DataContext as MangaSetting;
      try
      {
        if (setting != null)
          setting.Save();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
    }

    private void ChangeFolder_OnClick(object sender, RoutedEventArgs e)
    {
      var dialog = new VistaFolderBrowserDialog();
      if (dialog.ShowDialog() == true)
        this.FolderPath.Text = dialog.SelectedPath + System.IO.Path.DirectorySeparatorChar;
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
      var setting = this.DataContext as MangaSetting;
      if (setting != null)
        setting.Update();

      this.DataContext = null;
      this.DataContext = setting;
    }
  }
}
