using System.ComponentModel;

namespace MangaReader.Avalonia.ViewModel.Explorer.Tabs
{
  public class ExplorerTabViewModel : ViewModelBase
  {
    private string name;
    private int priority;
    private INotifyPropertyChanged content;

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

    public INotifyPropertyChanged Content
    {
      get { return content; }
      set { RaiseAndSetIfChanged(ref content, value); }
    }
  }
}