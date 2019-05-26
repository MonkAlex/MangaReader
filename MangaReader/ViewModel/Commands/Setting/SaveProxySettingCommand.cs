using System.Threading.Tasks;
using MangaReader.Core.Account;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Setting;

namespace MangaReader.ViewModel.Commands.Setting
{
  public class SaveProxySettingCommand : BaseCommand
  {
    private readonly ProxySettingModel proxySettingModel;

    public override async Task Execute(object parameter)
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = await context.Get<ProxySetting>().SingleAsync(s => s.Id == proxySettingModel.Id).ConfigureAwait(true);
        setting.Name = proxySettingModel.Name;
        setting.Address = proxySettingModel.Address;
        setting.UserName = proxySettingModel.UserName;
        setting.Password = proxySettingModel.Password;
        setting.SettingType = proxySettingModel.SettingType;
        await context.Save(setting).ConfigureAwait(true);
      }

      this.proxySettingModel.IsSaved = true;
      this.proxySettingModel.Close();
    }

    public SaveProxySettingCommand(ProxySettingModel proxySettingModel)
    {
      this.proxySettingModel = proxySettingModel;
      this.Name = "Сохранить";
    }
  }
}
