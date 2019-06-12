using System.Linq;
using System.Threading.Tasks;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class SettingTabViewModel : ExplorerTabViewModel
  {
    public bool HideTab { get; set; } = true;

    public override async Task OnUnselected(ExplorerTabViewModel newModel)
    {
      await base.OnUnselected(newModel).ConfigureAwait(true);
      if (!(newModel is SettingTabViewModel))
      {
        foreach (var tab in ExplorerViewModel.Instance.Tabs.OfType<SettingTabViewModel>().Where(t => t.HideTab).ToList())
          ExplorerViewModel.Instance.Tabs.Remove(tab);
      }
    }
  }
}
