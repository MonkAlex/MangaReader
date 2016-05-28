using System.Collections.Generic;
using MangaReader.Core.Services;
using MangaReader.Services;

namespace MangaReader.ViewModel.Setting
{
  public class MangaSettingModel : SettingViewModel
  {
    private readonly MangaSetting mangaSetting;
    private Compression.CompressionMode defaultCompression;
    private bool onlyUpdate;
    private bool compressManga;
    private string folder;
    private string login;
    private string password;

    internal int LoginId { get; }

    public List<Compression.CompressionMode> CompressionModes { get; private set; }

    public Compression.CompressionMode DefaultCompression
    {
      get { return defaultCompression; }
      set
      {
        defaultCompression = value;
        OnPropertyChanged();
      }
    }

    public bool OnlyUpdate
    {
      get { return onlyUpdate; }
      set
      {
        onlyUpdate = value;
        OnPropertyChanged();
      }
    }

    public bool CompressManga
    {
      get { return compressManga; }
      set
      {
        compressManga = value;
        OnPropertyChanged();
      }
    }

    public string Folder
    {
      get { return folder; }
      set
      {
        folder = value;
        OnPropertyChanged();
      }
    }

    public string Login
    {
      get { return login; }
      set
      {
        login = value;
        OnPropertyChanged();
      }
    }

    public string Password
    {
      get { return password; }
      set
      {
        password = value;
        OnPropertyChanged();
      }
    }

    public override void Save()
    {
      base.Save();

      this.mangaSetting.CompressManga = this.CompressManga;
      this.mangaSetting.DefaultCompression = this.DefaultCompression;
      this.mangaSetting.Folder  = this.Folder;
      this.mangaSetting.OnlyUpdate = this.OnlyUpdate;
      this.mangaSetting.Login.Name = this.Login;
      this.mangaSetting.Login.Password = this.Password;
    }

    public MangaSettingModel(MangaSetting setting)
    {
      this.mangaSetting = setting;
      this.Header = setting.MangaName;
      this.LoginId = this.mangaSetting.Login.Id;
      this.CompressionModes = Generic.GetEnumValues<Compression.CompressionMode>();

      this.CompressManga = this.mangaSetting.CompressManga;
      this.DefaultCompression = this.mangaSetting.DefaultCompression;
      this.Folder = this.mangaSetting.Folder;
      this.OnlyUpdate = this.mangaSetting.OnlyUpdate;
      this.Login = this.mangaSetting.Login.Name;
      this.Password = this.mangaSetting.Login.Password;
    }
  }
}