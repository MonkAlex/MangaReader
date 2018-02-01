using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Avalonia.ViewModel
{
  public class MangaModel : ViewModelBase
  {
    #region MangaProperties

    internal IManga ContextManga { get; set; }

    private string name;
    private string folder;
    private bool canChangeName;
    private bool nameIsReadonly;
    private bool? needCompress;
    private Compression.CompressionMode? compressionMode;

    public int Id { get; set; }

    public string Name
    {
      get => CanChangeName ? name : OriginalName;
      set => RaiseAndSetIfChanged(ref name, value);
    }

    public string OriginalName { get; protected set; }

    public string Folder
    {
      get => folder;
      set => RaiseAndSetIfChanged(ref folder, value);
    }

    public bool CanChangeName
    {
      get => canChangeName;
      set
      {
        RaiseAndSetIfChanged(ref canChangeName, value);
        this.NameIsReadonly = !value;
        RaisePropertyChanged(nameof(Name));
      }
    }

    public bool NameIsReadonly
    {
      get => nameIsReadonly;
      private set => RaiseAndSetIfChanged(ref nameIsReadonly, value);
    }

    public bool? NeedCompress
    {
      get => needCompress;
      set => RaiseAndSetIfChanged(ref needCompress, value);
    }

    public List<Compression.CompressionMode> CompressionModes { get; protected set; }

    public Compression.CompressionMode? CompressionMode
    {
      get => compressionMode;
      set => RaiseAndSetIfChanged(ref compressionMode, value);
    }

    private string type;
    private string completedIcon;
    private string needUpdateIcon;
    private double downloaded;
    private string speed;
    private string status;

    public string Type
    {
      get => type;
      set => RaiseAndSetIfChanged(ref type, value);
    }

    public bool IsCompleted { get; set; }

    public string CompletedIcon
    {
      get => completedIcon;
      set => RaiseAndSetIfChanged(ref completedIcon, value);
    }

    public string NeedUpdateIcon
    {
      get => needUpdateIcon;
      set => RaiseAndSetIfChanged(ref needUpdateIcon, value);
    }

    public bool NeedUpdate { get; set; }

    public double Downloaded
    {
      get => downloaded;
      set => RaiseAndSetIfChanged(ref downloaded, value);
    }

    public string Speed
    {
      get => speed;
      set => RaiseAndSetIfChanged(ref speed, value);
    }

    public string Status
    {
      get => status;
      set => RaiseAndSetIfChanged(ref status, value);
    }

    public DateTime? Created
    {
      get => created;
      set => RaiseAndSetIfChanged(ref created, value);
    }

    public DateTime? DownloadedAt
    {
      get => downloadedAt;
      set => RaiseAndSetIfChanged(ref downloadedAt, value);
    }

    public int SettingsId { get; set; }

    #endregion

    public void UpdateProperties(IManga manga)
    {
      if (manga == null)
        return;

      this.Id = manga.Id;
      this.Name = manga.Name;
      this.OriginalName = manga.ServerName;
      this.Folder = manga.Folder;
      this.CanChangeName = manga.IsNameChanged;
      this.NeedCompress = manga.NeedCompress;
      this.CompressionModes = new List<Compression.CompressionMode>(manga.AllowedCompressionModes);
      this.CompressionMode = manga.CompressionMode;
      if (manga.Downloaded > Downloaded)
        this.Downloaded = manga.Downloaded;
      SetCompletedIcon(manga.IsCompleted);
      SetType(manga);
      SetNeedUpdate(manga.NeedUpdate);
      this.Status = manga.Status;
      this.SettingsId = manga.Setting.Id;
      this.Created = manga.Created;
      this.DownloadedAt = manga.DownloadedAt;
    }

    private void SetCompletedIcon(bool isCompleted)
    {
      var result = "pack://application:,,,/Icons/play.png";
      switch (isCompleted)
      {
        case true:
          result = "pack://application:,,,/Icons/stop.png";
          break;

        case false:
          result = "pack://application:,,,/Icons/play.png";
          break;
      }
      this.IsCompleted = isCompleted;
      this.CompletedIcon = result;
    }

    private void SetType(IManga manga)
    {
      var result = "NA";
      var plugin = ConfigStorage.Plugins.SingleOrDefault(p => p.MangaType == manga.GetType());
      if (plugin != null)
        result = plugin.ShortName;
      this.Type = result;
    }

    private void SetNeedUpdate(bool needUpdate)
    {
      var result = "pack://application:,,,/Icons/play.png";
      switch (needUpdate)
      {
        case true:
          result = "pack://application:,,,/Icons/yes.png";
          break;

        case false:
          result = "pack://application:,,,/Icons/no.png";
          break;
      }
      this.NeedUpdate = needUpdate;
      this.NeedUpdateIcon = result;
    }

    private DateTime? created;
    private DateTime? downloadedAt;

    /*
    public ICommand Save => new MangaSaveCommand(this, WindowHelper.Library);

    public override void Show()
    {
      base.Show();

      var window = ViewService.Instance.TryGet<System.Windows.Window>(this);
      if (window != null)
      {
        window.Closing += (sender, args) => ViewService.Instance.TryRemove(this);
        window.ShowDialog();
      }
    }

    public void Close()
    {
      var window = ViewService.Instance.TryGet<System.Windows.Window>(this);
      if (window != null)
      {
        window.Close();
      }
    }
    */
    public MangaModel(IManga manga)
    {
      UpdateProperties(manga);
    }
  }
}