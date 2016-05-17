namespace MangaReader.UI.MainForm
{
  /// <summary>
  /// Interaction logic for BaseForm.xaml
  /// </summary>
  public class BaseForm : System.Windows.Controls.Page
  {
    public BaseForm()
    {
      ViewModel.Primitive.BaseViewModel.SubToViewModel(this);
    }
  }
}
