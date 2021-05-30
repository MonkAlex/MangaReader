using System.Threading.Tasks;
using System.Windows.Input;
using MangaReader.Avalonia.ViewModel.Command;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class ChangelogViewModel : ExplorerTabViewModel
  {
    private string content;

    public string Content
    {
      get { return content; }
      set
      {
        RaiseAndSetIfChanged(ref content, value);
      }
    }

    public ICommand HyperlinkCommand { get; set; }

    public override async Task OnSelected(ExplorerTabViewModel previousModel)
    {
      await base.OnSelected(previousModel).ConfigureAwait(true);

      if (string.IsNullOrWhiteSpace(Content))
        Content = MangaReader.Core.Update.VersionHistory.GetHistory();
    }

    public ChangelogViewModel()
    {
      this.Name = "История";
      this.Priority = 9999;
      this.HyperlinkCommand = new ChangelogNavigationCommand();
    }
  }
}
