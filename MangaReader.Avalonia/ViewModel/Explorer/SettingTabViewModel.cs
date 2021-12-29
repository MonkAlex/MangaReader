using System.Linq;
using System.Threading.Tasks;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public abstract class SettingTabViewModel : ExplorerTabViewModel
  {
    protected bool Child { get; set; } = true;
    protected readonly INavigator navigator;

    public override async Task OnUnselected(ExplorerTabViewModel newModel)
    {
      await base.OnUnselected(newModel).ConfigureAwait(true);

      // When select non-settings tab, remove all child
      if (!(newModel is SettingTabViewModel))
      {
        foreach (var tab in navigator.Get<SettingTabViewModel>().Where(t => t.Child).ToList())
          navigator.Remove(tab);
      }
    }

    protected SettingTabViewModel(INavigator navigator)
    {
      this.navigator = navigator;
    }
  }
}
