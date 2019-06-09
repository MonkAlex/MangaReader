using System;
using MangaReader.Core.Account;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class ProxySettingModel : ViewModelBase
  {
    public ProxySettingType SettingType { get; set; }

    public int Id
    {
      get => id;
      set => this.RaiseAndSetIfChanged(ref id, value);
    }

    private int id;

    public string Name
    {
      get => name;
      set => this.RaiseAndSetIfChanged(ref name, value);
    }

    private string name;

    public Uri Address
    {
      get => address;
      set => this.RaiseAndSetIfChanged(ref address, value);
    }

    private Uri address;

    public string UserName
    {
      get => userName;
      set => this.RaiseAndSetIfChanged(ref userName, value);
    }

    private string userName;

    public string Password
    {
      get => password;
      set => this.RaiseAndSetIfChanged(ref password, value);
    }

    private string password;

    public bool IsManual { get; private set; }

    public ProxySettingModel(ProxySetting setting)
    {
      Id = setting.Id;
      Name = setting.Name;
      Address = setting.Address;
      UserName = setting.UserName;
      Password = setting.Password;
      SettingType = setting.SettingType;
      IsManual = setting.SettingType == ProxySettingType.Manual;
    }

  }
}
