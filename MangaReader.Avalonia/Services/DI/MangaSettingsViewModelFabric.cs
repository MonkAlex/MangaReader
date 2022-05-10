using MangaReader.Avalonia.ViewModel;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Account;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia.Services
{
  /// <summary>
  /// Маппинг <see cref="MangaSetting"/> -> <see cref="MangaSettingsViewModel"/>
  /// </summary>
  public class MangaSettingsViewModelFabric : IFabric<MangaSetting, MangaSettingsViewModel>
  {
    private readonly INavigator navigator;
    private readonly IFabric<ProxySetting, ProxySettingModel> fabric;

    public MangaSettingsViewModel Create(MangaSetting input)
    {
      return new MangaSettingsViewModel(input, navigator, fabric);
    }

    public MangaSettingsViewModelFabric(INavigator navigator, IFabric<ProxySetting, ProxySettingModel> fabric)
    {
      this.navigator = navigator;
      this.fabric = fabric;
    }
  }
 
}
