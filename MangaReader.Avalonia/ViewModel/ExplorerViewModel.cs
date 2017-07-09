using System.Collections.ObjectModel;
using System.Linq;
using MangaReader.Avalonia.ViewModel.Explorer.Tabs;

namespace MangaReader.Avalonia.ViewModel
{
  public class ExplorerViewModel : ViewModelBase
  {
    public ObservableCollection<ExplorerTabViewModel> Tabs { get; }

    public ExplorerTabViewModel SelectedTab { get; set; }

    public void SelectDefaultTab()
    {
      this.SelectedTab = this.Tabs.OrderBy(t => t.Priority).FirstOrDefault();
    }

    public ExplorerViewModel()
    {
      this.Tabs = new ObservableCollection<ExplorerTabViewModel>();
      this.Tabs.Add(new LibraryTabViewModel());
    }
  }
}