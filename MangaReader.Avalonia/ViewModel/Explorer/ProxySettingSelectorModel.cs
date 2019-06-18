using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MangaReader.Avalonia.ViewModel.Command;
using MangaReader.Core.Account;
using MangaReader.Core.Exception;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class ProxySettingSelectorModel : SettingTabViewModel
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

    public ICommand UndoChanged { get; }

    public override Task OnSelected(ExplorerTabViewModel previousModel)
    {
      ReloadData();
      return base.OnSelected(previousModel);
    }

    private void ReloadData()
    {
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
      }, () => this.SelectedProxySettingModel?.IsManual == true);
      this.Test = new DelegateCommand(TestAddressImpl, () => !string.IsNullOrWhiteSpace(this.TestAddress));
      this.Save = new DelegateCommand(SaveImpl, () => true);
      this.UndoChanged = new DelegateCommand(ReloadData);

      this.PropertyChanged += (sender, args) =>
      {
        if (args.PropertyName == nameof(SelectedProxySettingModel))
          this.Remove.OnCanExecuteChanged();
        if (args.PropertyName == nameof(TestAddress))
          this.Test.OnCanExecuteChanged();
      };
      this.testAddress = "https://github.com/MonkAlex/MangaReader";
    }

    private async Task SaveImpl()
    {
      if (ProxySettingModels == null)
        return;

      try
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
      catch (MangaReaderException e)
      {
        ReloadData();
        await Services.Dialogs.ShowInfo("Сохранение прокси", "Произошла ошибка. \r\n" + e.Message);
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
        await Services.Dialogs.ShowInfo("Проверка прокси", "Успешно.");
      }
      catch (Exception e)
      {
        await Services.Dialogs.ShowInfo("Проверка прокси", "Произошла ошибка. \r\n" + e.Message);
      }
    }

    private class TestCoockieClient : CookieClient
    {

    }
  }
}
