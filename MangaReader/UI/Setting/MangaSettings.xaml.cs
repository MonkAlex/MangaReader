using System.Windows;
using System.Windows.Controls;
using Ookii.Dialogs.Wpf;

namespace MangaReader.UI.Setting
{
  /// <summary>
  /// Interaction logic for MangaSettings.xaml
  /// </summary>
  public partial class MangaSettings : UserControl
  {
    public MangaSettings()
    {
      InitializeComponent();
      ViewModel.Primitive.BaseViewModel.SubToViewModel(this);
    }

    private void ChangeFolder_OnClick(object sender, RoutedEventArgs e)
    {
      var dialog = new VistaFolderBrowserDialog();
      if (dialog.ShowDialog() == true)
        this.FolderPath.Text = dialog.SelectedPath + System.IO.Path.DirectorySeparatorChar;
    }
  }
}
