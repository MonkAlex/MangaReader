using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Account;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel.Setting
{
  public class ProxySettingSelectorModel : SettingViewModel
  {
    public ProxySettingModel SelectedProxySettingModel
    {
      get => selectedProxySettingModel;
      set
      {
        if (!Equals(selectedProxySettingModel, value))
        {
          selectedProxySettingModel = value;
          OnPropertyChanged();
        }
      }
    }

    private ProxySettingModel selectedProxySettingModel;

    public ObservableCollection<ProxySettingModel> ProxySettingModels
    {
      get => proxySettingModels;
      set
      {
        proxySettingModels = value;
        OnPropertyChanged();
      }
    }

    private ObservableCollection<ProxySettingModel> proxySettingModels;

    public override void Load()
    {
      base.Load();

      using (var context = Repository.GetEntityContext())
      {
        this.ProxySettingModels = new ObservableCollection<ProxySettingModel>(context
          .Get<ProxySetting>()
          .Select(s => new ProxySettingModel(s)));
        this.SelectedProxySettingModel = this.ProxySettingModels.FirstOrDefault();
      }
    }

    public ProxySettingSelectorModel()
    {
      this.Header = "Прокси";
    }

    public override async Task Save()
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = await context.Get<ProxySetting>().SingleAsync(s => s.Id == SelectedProxySettingModel.Id).ConfigureAwait(true);
        setting.Name = SelectedProxySettingModel.Name;
        setting.Address = SelectedProxySettingModel.Address;
        setting.UserName = SelectedProxySettingModel.UserName;
        setting.Password = SelectedProxySettingModel.Password;
        setting.SettingType = SelectedProxySettingModel.SettingType;
        await context.Save(setting).ConfigureAwait(true);
      }

      this.SelectedProxySettingModel.IsSaved = true;
    }
  }
}
