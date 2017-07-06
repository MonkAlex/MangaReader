using System.Collections.ObjectModel;
using MangaReader.Avalonia.ViewModel.Explorer;

namespace MangaReader.Avalonia.ViewModel
{
  public class ExplorerViewModel : ViewModelBase
  {
    public ObservableCollection<ExplorerTabViewModel<ViewModelBase>> Tabs { get; }

    public ExplorerViewModel()
    {
      this.Tabs = new ObservableCollection<ExplorerTabViewModel<ViewModelBase>>();
    }
  }
}