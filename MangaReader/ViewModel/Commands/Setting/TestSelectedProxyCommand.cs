using System;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using MangaReader.Core.Account;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Setting;

namespace MangaReader.ViewModel.Commands.Setting
{
  public class TestSelectedProxyCommand : BaseCommand
  {
    private readonly ProxySettingSelectorModel model;

    public override bool CanExecute(object parameter)
    {
      return base.CanExecute(parameter) && !string.IsNullOrWhiteSpace(model.TestAddress);
    }

    public override async Task Execute(object parameter)
    {
      try
      {
        var selected = model.SelectedProxySettingModel;
        var address = model.TestAddress;

        var setting = new ProxySetting(selected.SettingType)
        {
          Address = selected.Address, UserName = selected.UserName, Password = selected.Password
        };
        var proxy = setting.GetProxy();
        var client = new CookieClient(new CookieContainer()) { Proxy = proxy };
        await client.DownloadStringTaskAsync(address).ConfigureAwait(true);
        Dialogs.ShowInfo("Проверка прокси", "Успешно.");
      }
      catch (Exception e)
      {
        Dialogs.ShowInfo("Проверка прокси", "Произошла ошибка. \r\n" + e.Message);
      }
    }

    public TestSelectedProxyCommand(ProxySettingSelectorModel model)
    {
      this.model = model;
      this.model.PropertyChanged += ModelOnPropertyChanged;
    }

    private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(model.TestAddress))
        OnCanExecuteChanged();
    }
  }
}
