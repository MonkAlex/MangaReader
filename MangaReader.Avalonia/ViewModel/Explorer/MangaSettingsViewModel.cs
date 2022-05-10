﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Avalonia.ViewModel.Command;
using MangaReader.Core.Services;
using System.Windows.Input;
using MangaReader.Core.Account;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Avalonia.Services;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class MangaSettingsViewModel : SettingTabViewModel
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

    public ProxySettingModel SelectedProxySettingModel
    {
      get
      {
        return selectedProxySettingModel;
      }

      set
      {
        this.RaiseAndSetIfChanged(ref selectedProxySettingModel, value);
        if (selectedProxySettingModel != null)
          proxySettingId = selectedProxySettingModel.Id;
      }
    }

    private ProxySettingModel selectedProxySettingModel;
    private int proxySettingId;

    public IEnumerable<ProxySettingModel> ProxySettingModels
    {
      get => proxySettingModels;
      set => this.RaiseAndSetIfChanged(ref proxySettingModels, value);
    }

    private IEnumerable<ProxySettingModel> proxySettingModels;

    public ICommand Save { get; }

    public ICommand UndoChanged { get; }

    private readonly IFabric<ProxySetting, ProxySettingModel> proxySettingFabric;

    private async Task ReloadConfig()
    {
      using (var context = Repository.GetEntityContext())
      {
        var setting = await context.Get<MangaSetting>().SingleOrDefaultAsync(s => s.Id == mangaSettingsId).ConfigureAwait(true);
        if (setting != null)
        {
          this.Compress = setting.CompressManga;
          this.OnlyUpdate = setting.OnlyUpdate;
          this.Folder = setting.Folder;
          this.Compression = setting.DefaultCompression;
          this.FolderNamingStrategy = FolderNamingStrategies.FirstOrDefault(s => s.Id == setting.FolderNamingStrategy);
          this.MainUri = setting.MainUri.OriginalString;
          this.ProxySettingModels = await context
            .Get<ProxySetting>()
            .Select(s => proxySettingFabric.Create(s))
            .ToListAsync()
            .ConfigureAwait(true);
          this.proxySettingId = setting.ProxySetting.Id;
          this.SelectedProxySettingModel = this.ProxySettingModels.FirstOrDefault(m => m.Id == proxySettingId);
        }
      }
    }

    private async Task SaveConfig()
    {
      try
      {
        using (var context = Repository.GetEntityContext())
        {
          var setting = await context.Get<MangaSetting>()
            .SingleOrDefaultAsync(s => s.Id == mangaSettingsId)
            .ConfigureAwait(true);
          if (setting != null)
          {
            setting.CompressManga = this.Compress == true;
            setting.OnlyUpdate = this.OnlyUpdate == true;
            setting.Folder = this.Folder;
            setting.DefaultCompression = this.Compression;
            setting.FolderNamingStrategy = this.FolderNamingStrategy.Id;
            if (Uri.TryCreate(this.MainUri, UriKind.Absolute, out Uri parsedUri) && parsedUri != setting.MainUri)
              setting.MainUri = parsedUri;
            if (proxySettingId != setting.ProxySetting.Id)
              setting.ProxySetting = await context.Get<ProxySetting>().SingleAsync(s => s.Id == proxySettingId).ConfigureAwait(false);
            await context.Save(setting).ConfigureAwait(true);
          }
        }
      }
      catch (Exception ex)
      {
        Log.Exception(ex);
        await Services.Dialogs.ShowInfo("Не удалось сохранить настройки", ex.Message);
      }
    }

    public MangaSettingsViewModel(MangaSetting setting, INavigator navigator, IFabric<ProxySetting, ProxySettingModel> proxySettingFabric) : base(navigator)
    {
      this.proxySettingFabric = proxySettingFabric;
      this.Name = setting.MangaName;
      this.Priority = 500;
      this.mangaSettingsId = setting.Id;
      this.FolderNamingStrategies = Core.Services.FolderNamingStrategies.Strategies.ToList();
      this.FolderNamingStrategies.Insert(0, new BaseFolderNameStrategy("Использовать общие настройки"));

      ReloadConfig().LogException();
      this.Save = new DelegateCommand(SaveConfig, () => true);
      this.UndoChanged = new DelegateCommand(ReloadConfig, () => true);
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
