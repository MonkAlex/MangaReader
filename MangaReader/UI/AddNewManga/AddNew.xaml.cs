using System.Windows;
using MangaReader.Services;

namespace MangaReader.UI.AddNewManga
{
  /// <summary>
  /// Interaction logic for AddNew.xaml
  /// </summary>
  public partial class AddNew : Window
  {
    public AddNew()
    {
      InitializeComponent();
      Owner = WindowHelper.Owner;
      ViewModel.Primitive.BaseViewModel.SubToViewModel(this);
    }
  }
}
