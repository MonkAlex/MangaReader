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

    public LoginModel Login { get; }

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

    public override void Save()
    {
      base.Save();

      this.mangaSetting.CompressManga = this.CompressManga;
      this.mangaSetting.DefaultCompression = this.DefaultCompression;
      this.mangaSetting.Folder = this.Folder;
      this.mangaSetting.OnlyUpdate = this.OnlyUpdate;
      this.Login.Save();
    }

    public MangaSettingModel(MangaSetting setting)
    {
      this.mangaSetting = setting;
      this.Header = setting.MangaName;
      this.CompressionModes = Generic.GetEnumValues<Compression.CompressionMode>();

      this.CompressManga = this.mangaSetting.CompressManga;
      this.DefaultCompression = this.mangaSetting.DefaultCompression;
      this.Folder = this.mangaSetting.Folder;
      this.OnlyUpdate = this.mangaSetting.OnlyUpdate;
      this.Login = new LoginModel(this.mangaSetting.Login) {IsEnabled = true};
    }
  }
}