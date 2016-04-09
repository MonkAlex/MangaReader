using System.Linq;
using System.Windows.Data;
using MangaReader.Manga;
using MangaReader.Services;
using MangaReader.Services.Config;
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
  }
}
