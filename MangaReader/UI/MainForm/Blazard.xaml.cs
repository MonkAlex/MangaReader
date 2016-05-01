using MangaReader.Services;

namespace MangaReader.UI.MainForm
{
  /// <summary>
  /// Interaction logic for Blazard.xaml
  /// </summary>
  public partial class Blazard : BaseForm
  {

    public Blazard()
    {
      InitializeComponent();
      this.BlazardManga.DataContext = Library.SelectedManga;
    }
  }
}
