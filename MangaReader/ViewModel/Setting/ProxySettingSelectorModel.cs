using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MangaReader.Core.Account;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.ViewModel.Commands.Setting;
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

    public ICommand Add { get; }

    public ICommand Remove { get; }

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
      this.Add = new AddNewProxyCommand(this);
      this.Remove = new RemoveSelectedProxyCommand(this);
    }

    public override async Task Save()
    {
      using (var context = Repository.GetEntityContext())
      {
        var manualProxies = await context.Get<ProxySetting>().ToListAsync().ConfigureAwait(true);
        foreach (var model in ProxySettingModels.Where(m => m.IsManual))
        {
          var setting = model.Id == 0 ? new ProxySetting(ProxySettingType.Manual) : manualProxies.Single(p => p.Id == model.Id);
          setting.Name = model.Name;
          setting.Address = model.Address;
          setting.UserName = model.UserName;
          setting.Password = model.Password;
          setting.SettingType = model.SettingType;
          await context.Save(setting).ConfigureAwait(true);
          model.Id = setting.Id;
        }

        var toRemove = manualProxies.Where(p => ProxySettingModels.All(m => m.Id != p.Id)).ToList();
        foreach (var setting in toRemove)
        {
          await context.Delete(setting).ConfigureAwait(true);
        }
      }
    }
  }
}
