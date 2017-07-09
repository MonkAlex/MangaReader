namespace MangaReader.Avalonia.ViewModel.Explorer.Tabs
{
  public class SearchTabViewModel : ExplorerTabViewModel
  {
    public SearchTabViewModel()
    {
      Name = "Search";
      Priority = 20;
      Content = new SearchContentViewModel();
    }
  }
}