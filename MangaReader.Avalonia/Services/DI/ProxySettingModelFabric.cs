using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core.Account;

namespace MangaReader.Avalonia.Services
{
  /// <summary>
  /// Маппинг <see cref="ProxySetting"/> -> <see cref="ProxySettingModel"/>
  /// </summary>
  public class ProxySettingModelFabric : IFabric<ProxySetting, ProxySettingModel>
  {
    public ProxySettingModel Create(ProxySetting input)
    {
      return new ProxySettingModel(input);
    }
  }
 
}
