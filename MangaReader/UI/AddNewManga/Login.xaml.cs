using System.Windows.Controls;
using System.Windows.Input;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.UI.AddNewManga
{
  /// <summary>
  /// Interaction logic for Login.xaml
  /// </summary>
  public partial class Login : UserControl
  {
    public Login()
    {
      InitializeComponent();
      BaseViewModel.SubToViewModel(this);
    }

    private void Bookmarks_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
      var listBox = sender as ListBox;
      if (listBox == null || listBox.SelectedItems.Count == 0)
        return;

      if (!(e.MouseDevice.DirectlyOver is TextBlock))
        listBox.SelectedIndex = -1;
    }
  }
}
