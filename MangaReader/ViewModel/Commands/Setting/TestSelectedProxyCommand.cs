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
        var address = new Uri(model.TestAddress);

        var setting = new ProxySetting(selected.SettingType)
        {
          Address = selected.Address, UserName = selected.UserName, Password = selected.Password
        };
        var proxy = setting.GetProxy();

        var client = SiteHttpClientFactory.Get(address, proxy, new CookieContainer());
        var page = await client.GetPage(address).ConfigureAwait(true);
        if (page.HasContent)
          Dialogs.ShowInfo("Проверка прокси", "Успешно.");
        else
          Dialogs.ShowInfo("Проверка прокси", "Не удалось получить страницу, проверьте настройки.\r\n" + page.Error);
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
