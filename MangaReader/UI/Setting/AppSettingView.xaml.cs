using System.Windows.Controls;

namespace MangaReader.UI.Setting
{
  /// <summary>
  /// Interaction logic for AppSettingView.xaml
  /// </summary>
  public partial class AppSettingView : UserControl
  {
    public AppSettingView()
    {
      InitializeComponent();
      ViewModel.Primitive.BaseViewModel.SubToViewModel(this);
    }
  }
}
