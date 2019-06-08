using System.Threading.Tasks;
using MangaReader.Core.Account;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Setting;

namespace MangaReader.ViewModel.Commands.Setting
{
  public class AddNewProxyCommand : BaseCommand
  {
    private readonly ProxySettingSelectorModel model;

    public override Task Execute(object parameter)
    {
      var newProxy = new ProxySettingModel(new ProxySetting(ProxySettingType.Manual));
      model.ProxySettingModels.Add(newProxy);
      model.SelectedProxySettingModel = newProxy;
      return Task.CompletedTask;
    }

    public AddNewProxyCommand(ProxySettingSelectorModel model)
    {
      this.model = model;
    }
  }
}
