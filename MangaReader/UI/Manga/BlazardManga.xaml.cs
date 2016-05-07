using System.Windows.Controls;

namespace MangaReader.UI.Manga
{
  /// <summary>
  /// Interaction logic for BlazardManga.xaml
  /// </summary>
  public partial class BlazardManga : UserControl
  {
    public BlazardManga()
    {
      InitializeComponent();
      ViewModel.Primitive.BaseViewModel.SubToViewModel(this);
    }
  }
}
