using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;

namespace MangaReader.ViewModel.Setting
{
  public class MangaSettingModel : SettingViewModel
  {
    private int id;
    private Compression.CompressionMode defaultCompression;
    private bool onlyUpdate;
    private bool compressManga;
    private string folder;
    private string mainUri;

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

    public string MainUri
    {
      get { return mainUri; }
      set
      {
        mainUri = value;
        OnPropertyChanged();
      }
    }

    public FolderNamingModel FolderNamingStrategy { get; set; }

    public ProxySettingSelectorModel ProxySettingSelector { get; private set; }

    public override async Task Save()
    {
      using (var context = Repository.GetEntityContext($"Save settings for {Header}"))
      {
        var setting = await context.Get<MangaSetting>().SingleAsync(s => s.Id == id).ConfigureAwait(true);
        setting.CompressManga = this.CompressManga;
        setting.DefaultCompression = this.DefaultCompression;
        setting.Folder = this.Folder;
        setting.OnlyUpdate = this.OnlyUpdate;
        setting.FolderNamingStrategy = this.FolderNamingStrategy.Selected.Id;
        if (Uri.TryCreate(this.MainUri, UriKind.Absolute, out Uri parsedUri) && parsedUri != setting.MainUri)
          setting.MainUri = parsedUri;
        await this.Login.Save().ConfigureAwait(true);
        await context.Save(setting).ConfigureAwait(true);
      }
    }

    public MangaSettingModel(MangaSetting setting, ObservableCollection<ProxySettingModel> proxySettingModels)
    {
      this.id = setting.Id;
      this.Header = setting.MangaName;
      this.CompressionModes = Generic.GetEnumValues<Compression.CompressionMode>();

      this.CompressManga = setting.CompressManga;
      this.DefaultCompression = setting.DefaultCompression;
      this.Folder = setting.Folder;
      this.OnlyUpdate = setting.OnlyUpdate;
      this.Login = new LoginModel(setting.Login) { IsEnabled = true };
      this.MainUri = setting.MainUri.OriginalString;

      this.FolderNamingStrategy = new FolderNamingModel();
      this.FolderNamingStrategy.Strategies.Insert(0, new FolderNamingStrategyDto() { Name = "Использовать общие настройки" });
      this.FolderNamingStrategy.SelectedGuid = setting.FolderNamingStrategy;

      this.ProxySettingSelector = new ProxySettingSelectorModel(setting.ProxySetting, proxySettingModels);
    }
  }
}
