namespace MangaReader.Avalonia.ViewModel.Explorer.Tabs
{
  public class LibraryTabViewModel : ExplorerTabViewModel
  {
    public LibraryTabViewModel()
    {
      Priority = 10;
      Name = "Мain";
      Content = new LibraryContentViewModel();
    }
  }
}