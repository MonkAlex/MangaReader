using System.Windows;
using MangaReader.ViewModel;
using MangaReader.ViewModel.Manga;

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
    }

    private void FrameworkElement_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var model = DataContext as MainPageModel;
      if (model == null)
        return;

      model.SelectedMangaModels.Remove(e.OldValue as MangaModel);
      model.SelectedMangaModels.Add(e.NewValue as MangaModel);
    }
  }
}
