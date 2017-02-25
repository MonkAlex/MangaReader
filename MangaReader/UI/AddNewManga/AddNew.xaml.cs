using System.Windows;
using System.Windows.Controls;
using MangaReader.Services;

namespace MangaReader.UI.AddNewManga
{
  /// <summary>
  /// Interaction logic for AddNew.xaml
  /// </summary>
  public partial class AddNew : ContentControl
  {
    public AddNew()
    {
      InitializeComponent();
      ViewModel.Primitive.BaseViewModel.SubToViewModel(this);
    }
  }
}
