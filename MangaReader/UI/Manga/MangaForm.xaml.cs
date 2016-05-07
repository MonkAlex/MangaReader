using System.Windows;
using MangaReader.Services;

namespace MangaReader.UI.Manga
{
  /// <summary>
  /// Логика взаимодействия для MangaForm.xaml
  /// </summary>
  public partial class MangaForm : Window
  {
    public MangaForm()
    {
      Owner = WindowHelper.Owner;
      InitializeComponent();
      ViewModel.Primitive.BaseViewModel.SubToViewModel(this);
    }
  }
}
