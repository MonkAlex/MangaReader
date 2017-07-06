namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class ExplorerTabViewModel<T> : ViewModelBase where T : ViewModelBase
  {
    private string name;
    private int priority;
    private T content;

    public string Name
    {
      get { return name; }
      set { RaiseAndSetIfChanged(ref name, value); }
    }

    public int Priority
    {
      get { return priority; }
      set { RaiseAndSetIfChanged(ref priority, value); }
    }

    public T Content
    {
      get { return content; }
      set { RaiseAndSetIfChanged(ref content, value); }
    }
  }
}