using System.Threading.Tasks;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class ExplorerTabViewModel : ViewModelBase
  {
    private string name;
    private int priority;

    public string ShortName
    {
      get => shortName;
      set
      {
        if (value?.Length > 11)
          value = value?.Substring(0, 9) + "...";
        this.RaiseAndSetIfChanged(ref shortName, value);
      }
    }

    private string shortName;

    public string Name
    {
      get { return name; }
      set
      {
        this.ShortName = value;
        RaiseAndSetIfChanged(ref name, value);
      }
    }

    public int Priority
    {
      get { return priority; }
      set { RaiseAndSetIfChanged(ref priority, value); }
    }

    public virtual Task OnSelected(ExplorerTabViewModel previousModel)
    {
      return Task.CompletedTask;
    }

    public virtual Task OnUnselected(ExplorerTabViewModel newModel)
    {
      return Task.CompletedTask;
    }
  }
}
