using System.Windows;

namespace MangaReader.UI
{
  /// <summary>
  /// Interaction logic for ShutdownWindow.xaml
  /// </summary>
  public partial class ShutdownWindow : Window
  {
    public ShutdownWindow()
    {
      InitializeComponent();
      ViewModel.Primitive.BaseViewModel.SubToViewModel(this);
      this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }
  }
}
