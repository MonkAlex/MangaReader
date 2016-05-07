using System.Windows;
using MangaReader.Services;
using MangaReader.ViewModel;

namespace MangaReader.UI.MainForm
{
  /// <summary>
  /// Interaction logic for BaseForm.xaml
  /// </summary>
  public partial class BaseForm : System.Windows.Controls.Page
  {
    // TODO : времянка для компилируемости.
    internal MainPageModel Model { get; set; }

    public BaseForm()
    {
      ViewModel.Primitive.BaseViewModel.SubToViewModel(this);
      this.Model = new MainPageModel();
      this.DataContext = Model;
      Command.AddMainMenuCommands(this);
      this.Initialized += (sender, args) => LibraryWPF.Initialize(this);
    }

    protected void FilterChanged(object sender, RoutedEventArgs e)
    {
      if (Model.View != null)
      {
        Model.View.Refresh();
      }
    }
  }
}
