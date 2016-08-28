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
  }
}
