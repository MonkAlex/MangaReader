using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using MangaReader.ViewModel;
using MangaReader.ViewModel.Manga;

namespace MangaReader.UI.MainForm
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

      if (!(sender is ListViewItem item))
        return;

      if (!(item.DataContext is MangaModel downloadable))
        return;

      var defaultCommand = downloadable.MangaMenu.FirstOrDefault(m => m.IsDefault);
      if (defaultCommand != null && defaultCommand.Command.CanExecute(downloadable))
        defaultCommand.Command.Execute(downloadable);
    }

    private void ListView_MouseDown(object sender, MouseButtonEventArgs e)
    {
      if (sender is ListView listView)
      {
        listView.SelectedIndex = -1;
      }
    }

    private void FormLibrary_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      var model = DataContext as MainPageModel;
      if (model == null)
        return;

      foreach (MangaModel item in e.RemovedItems)
        model.SelectedMangaModels.Remove(item);
      foreach (MangaModel item in e.AddedItems)
        model.SelectedMangaModels.Add(item);
    }
  }
  
}