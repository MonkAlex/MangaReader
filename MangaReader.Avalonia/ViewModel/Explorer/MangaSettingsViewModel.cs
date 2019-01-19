using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Avalonia.ViewModel.Command;
using MangaReader.Core.Services;
using System.Windows.Input;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class MangaSettingsViewModel : ExplorerTabViewModel
  {
    private readonly int mangaSettingsId;

    public bool? Compress
    {
      get => compress;
      set => RaiseAndSetIfChanged(ref compress, value);
    }

    private bool? compress;

    public bool? OnlyUpdate
    {
      get => onlyUpdate;
      set => RaiseAndSetIfChanged(ref onlyUpdate, value);
    }

    private bool? onlyUpdate;

    public string Folder
    {
      get => folder;
      set => RaiseAndSetIfChanged(ref folder, value);
    }

    private string folder;

    public IFolderNamingStrategy FolderNamingStrategy
    {
      get => folderNamingStrategy;
      set => RaiseAndSetIfChanged(ref folderNamingStrategy, value);
    }

    private IFolderNamingStrategy folderNamingStrategy;

    public List<IFolderNamingStrategy> FolderNamingStrategies { get; }

    public string MainUri
    {
      get => mainUri;
      set => RaiseAndSetIfChanged(ref mainUri, value);
    }

    private string mainUri;

    public List<Compression.CompressionMode> Compressions => Generic.GetEnumValues<Compression.CompressionMode>();

    public Compression.CompressionMode Compression
    {
      get => compression;
      set => RaiseAndSetIfChanged(ref compression, value);
    }

    private Compression.CompressionMode compression;

    public override async Task OnUnselected(ExplorerTabViewModel newModel)
    {
      await base.OnUnselected(newModel);
      if (!(newModel is SettingsViewModel || newModel is MangaSettingsViewModel))
      {
        foreach (var tab in ExplorerViewModel.Instance.Tabs.OfType<MangaSettingsViewModel>().ToList())
          ExplorerViewModel.Instance.Tabs.Remove(tab);
      }
    }

    public ICommand Save { get; }

    public ICommand UndoChanged { get; }

    private void ReloadConfig()
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = context.Get<MangaSetting>().SingleOrDefault(s => s.Id == mangaSettingsId);
        if (setting != null)
        {
          this.Compress = setting.CompressManga;
          this.OnlyUpdate = setting.OnlyUpdate;
          this.Folder = setting.Folder;
          this.Compression = setting.DefaultCompression;
          this.FolderNamingStrategy = FolderNamingStrategies.FirstOrDefault(s => s.Id == setting.FolderNamingStrategy);
          this.MainUri = setting.MainUri.OriginalString;
        }
      }
    }

    private void SaveConfig()
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = context.Get<MangaSetting>().SingleOrDefault(s => s.Id == mangaSettingsId);
        if (setting != null)
        {
          setting.CompressManga = this.Compress == true;
          setting.OnlyUpdate = this.OnlyUpdate == true;
          setting.Folder = this.Folder;
          setting.DefaultCompression = this.Compression;
          setting.FolderNamingStrategy = this.FolderNamingStrategy.Id;
          if (Uri.TryCreate(this.MainUri, UriKind.Absolute, out Uri parsedUri) && parsedUri != setting.MainUri)
            setting.MainUri = parsedUri;
          context.Save(setting);
        }
      }
    }

    public MangaSettingsViewModel(MangaSetting setting)
    {
      this.Name = setting.MangaName;
      this.Priority = 500;
      this.mangaSettingsId = setting.Id;
      this.FolderNamingStrategies = Core.Services.FolderNamingStrategies.Strategies.ToList();
      this.FolderNamingStrategies.Insert(0, new BaseFolderNameStrategy("Использовать общие настройки"));

      ReloadConfig();
      this.Save = new DelegateCommand(SaveConfig);
      this.UndoChanged = new DelegateCommand(ReloadConfig);
    }

    private class BaseFolderNameStrategy : IFolderNamingStrategy
    {
      public Guid Id { get; }
      public string Name { get; }
      public string FormateChapterFolder(Chapter chapter)
      {
        throw new NotImplementedException();
      }

      public string FormateVolumeFolder(Volume volume)
      {
        throw new NotImplementedException();
      }

      public BaseFolderNameStrategy(string name)
      {
        this.Name = name;
      }
    }
  }
}