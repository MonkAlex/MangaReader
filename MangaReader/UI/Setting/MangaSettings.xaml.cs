using System.Windows;
using System.Windows.Controls;
using MangaReader.Services;

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
      this.FolderPath.Text = Dialogs.SelectFolder(this.FolderPath.Text);
    }
  }
}
