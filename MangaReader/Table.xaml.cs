using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using MangaReader.UI.MainForm;
using MangaReader.ViewModel.Manga;

namespace MangaReader
{
  /// <summary>
  /// Логика взаимодействия для Table.xaml
  /// </summary>
  public partial class Table : BaseForm
  {
    public Table()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Обработчик двойного клика по манге.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Mangas_clicked(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount < 2)
        return;

      var item = sender as ListViewItem;
      if (item == null)
        return;

      var downloadable = item.DataContext as MangaViewModel;
      if (downloadable == null)
        return;

      var defaultCommand = downloadable.MangaMenu.FirstOrDefault(m => m.IsDefault);
      if (defaultCommand != null && defaultCommand.Command.CanExecute(downloadable))
        defaultCommand.Command.Execute(downloadable);
    }

    private void ListView_MouseDown(object sender, MouseButtonEventArgs e)
    {
      var listView = sender as ListView;
      if (listView != null)
      {
        listView.SelectedIndex = -1;
      }
    }
  }
  
}