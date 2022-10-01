using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MangaReader.Core;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
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
      var absolutePath = DirectoryHelpers.GetAbsoluteFolderPath(this.FolderPath.Text);
      var selectedPath = Dialogs.SelectFolder(absolutePath);
      var relativePath = DirectoryHelpers.GetRelativePath(Environments.Instance.WorkFolder, selectedPath);
      this.FolderPath.Text = relativePath.StartsWith(@"..\..\") ? selectedPath : relativePath;
    }
  }
}
