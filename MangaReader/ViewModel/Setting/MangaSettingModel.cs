using System.Collections.Generic;
using System.Linq;
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

    public FolderNamingModel FolderNamingStrategy { get; set; }

    public override void Save()
    {
      base.Save();

      using (var context = Repository.GetEntityContext())
      {
        var setting = context.Get<MangaSetting>().Single(s => s.Id == id);
        setting.CompressManga = this.CompressManga;
        setting.DefaultCompression = this.DefaultCompression;
        setting.Folder = this.Folder;
        setting.OnlyUpdate = this.OnlyUpdate;
        setting.FolderNamingStrategy = this.FolderNamingStrategy.Selected.Id;
        this.Login.Save();
        context.Save(setting);
      }
    }

    public MangaSettingModel(MangaSetting setting)
    {
      this.id = setting.Id;
      this.Header = setting.MangaName;
      this.CompressionModes = Generic.GetEnumValues<Compression.CompressionMode>();

      this.CompressManga = setting.CompressManga;
      this.DefaultCompression = setting.DefaultCompression;
      this.Folder = setting.Folder;
      this.OnlyUpdate = setting.OnlyUpdate;
      this.Login = new LoginModel(setting.Login) {IsEnabled = true};

      this.FolderNamingStrategy = new FolderNamingModel();
      this.FolderNamingStrategy.Strategies.Insert(0, new FolderNamingStrategyDto() {Name = "Использовать общие настройки"});
      this.FolderNamingStrategy.SelectedGuid = setting.FolderNamingStrategy;
    }
  }
}