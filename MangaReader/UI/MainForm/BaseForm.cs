using System.Windows;
using MangaReader.ViewModel;

namespace MangaReader.UI.MainForm
{
  /// <summary>
  /// Interaction logic for BaseForm.xaml
  /// </summary>
  public class BaseForm : System.Windows.Controls.Page
  {
    // TODO : времянка для компилируемости.
    private MainPageModel Model { get { return DataContext as MainPageModel; } }

    public BaseForm()
    {
      ViewModel.Primitive.BaseViewModel.SubToViewModel(this);
    }

    protected void FilterChanged(object sender, RoutedEventArgs e)
    {
      if (Model != null && Model.View != null)
        Model.View.Refresh();
    }
  }
}
