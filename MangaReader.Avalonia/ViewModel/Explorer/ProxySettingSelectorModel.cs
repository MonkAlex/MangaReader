using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MangaReader.Avalonia.ViewModel.Command;
using MangaReader.Core.Account;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class ProxySettingSelectorModel : ExplorerTabViewModel
  {

    public ProxySettingModel SelectedProxySettingModel
    {
      get => selectedProxySettingModel;
      set => this.RaiseAndSetIfChanged(ref selectedProxySettingModel, value);
    }

    private ProxySettingModel selectedProxySettingModel;

    public ObservableCollection<ProxySettingModel> ProxySettingModels
    {
      get => proxySettingModels;
      set => this.RaiseAndSetIfChanged(ref proxySettingModels, value);
    }

    private ObservableCollection<ProxySettingModel> proxySettingModels;

    public string TestAddress
    {
      get => testAddress;
      set => this.RaiseAndSetIfChanged(ref testAddress, value);
    }

    private string testAddress;

    public DelegateCommand Add { get; }

    public DelegateCommand Remove { get; }

    public DelegateCommand Test { get; }

    public ICommand Save { get; }

    public ICommand Undo { get; }

    public override Task OnSelected(ExplorerTabViewModel previousModel)
    {
      using (var context = Repository.GetEntityContext())
      {
        this.ProxySettingModels = new ObservableCollection<ProxySettingModel>(context
          .Get<ProxySetting>()
          .Select(s => new ProxySettingModel(s)));
        this.SelectedProxySettingModel = this.ProxySettingModels.FirstOrDefault();
      }
      return base.OnSelected(previousModel);
    }

    public ProxySettingSelectorModel()
    {
      this.Name = "Прокси";
      this.Priority = 600;
      this.Add = new DelegateCommand(() =>
      {
        var newProxy = new ProxySettingModel(new ProxySetting(ProxySettingType.Manual));
        this.ProxySettingModels.Add(newProxy);
        this.SelectedProxySettingModel = newProxy;
      });
      this.Remove = new DelegateCommand(() =>
      {
        var selected = this.SelectedProxySettingModel;
        var models = this.ProxySettingModels;
        var index = models.IndexOf(selected);
        var next = models.Count > index + 1 ? models[index + 1] : models[index - 1];
        models.Remove(selected);
        this.SelectedProxySettingModel = next;
        return Task.CompletedTask;
      }, () => this.SelectedProxySettingModel?.IsManual == true, SubscribeToCommand(nameof(SelectedProxySettingModel)));
      this.Test = new DelegateCommand(TestAddressImpl, () => !string.IsNullOrWhiteSpace(this.TestAddress), SubscribeToCommand(nameof(TestAddress)));
      this.Save = new DelegateCommand(SaveImpl, () => true);
      this.testAddress = "https://github.com/MonkAlex/MangaReader";
    }

    private async Task SaveImpl()
    {
      if (ProxySettingModels == null)
        return;

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

    private async Task TestAddressImpl()
    {
      try
      {
        var selected = this.SelectedProxySettingModel;
        var address = this.TestAddress;

        var setting = new ProxySetting(selected.SettingType)
        {
          Address = selected.Address,
          UserName = selected.UserName,
          Password = selected.Password
        };
        var proxy = setting.GetProxy();
        var client = new TestCoockieClient() { Proxy = proxy };
        await client.DownloadStringTaskAsync(address).ConfigureAwait(true);
        Log.Add("Успешно.");
      }
      catch (Exception e)
      {
        Log.Exception(e, "Произошла ошибка.");
      }
    }

    private class TestCoockieClient : CookieClient
    {

    }
  }
}
