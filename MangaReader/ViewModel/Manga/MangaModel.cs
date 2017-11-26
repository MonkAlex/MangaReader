using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.Services;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Commands.Manga;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel.Manga
{
  public class MangaModel : BaseViewModel
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
      get { return CanChangeName ? name : OriginalName; }
      set
      {
        name = value;
        OnPropertyChanged();
      }
    }

    public string OriginalName { get; protected set; }

    public string Folder
    {
      get { return folder; }
      set
      {
        folder = value;
        OnPropertyChanged();
      }
    }

    public bool CanChangeName
    {
      get { return canChangeName; }
      set
      {
        canChangeName = value;
        this.NameIsReadonly = !value;
        OnPropertyChanged();
        OnPropertyChanged(nameof(Name));
      }
    }

    public bool NameIsReadonly
    {
      get { return nameIsReadonly; }
      private set
      {
        nameIsReadonly = value;
        OnPropertyChanged();
      }
    }

    public bool? NeedCompress
    {
      get { return needCompress; }
      set
      {
        needCompress = value;
        OnPropertyChanged();
      }
    }

    public List<Compression.CompressionMode> CompressionModes { get; protected set; }

    public Compression.CompressionMode? CompressionMode
    {
      get { return compressionMode; }
      set
      {
        compressionMode = value;
        OnPropertyChanged();
      }
    }

    private string type;
    private string completedIcon;
    private string needUpdateIcon;
    private double downloaded;
    private string speed;
    private string status;

    public string Type
    {
      get { return type; }
      set
      {
        type = value;
        OnPropertyChanged();
      }
    }

    public bool IsCompleted { get; set; }

    public string CompletedIcon
    {
      get { return completedIcon; }
      set
      {
        completedIcon = value;
        OnPropertyChanged();
      }
    }

    public string NeedUpdateIcon
    {
      get { return needUpdateIcon; }
      set
      {
        needUpdateIcon = value;
        OnPropertyChanged();
      }
    }

    public bool NeedUpdate { get; set; }

    public double Downloaded
    {
      get { return downloaded; }
      set
      {
        downloaded = value;
        OnPropertyChanged();
      }
    }

    public string Speed
    {
      get { return speed; }
      set
      {
        speed = value;
        OnPropertyChanged();
      }
    }

    public string Status
    {
      get { return status; }
      set
      {
        status = value;
        OnPropertyChanged();
      }
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

    private ObservableCollection<ContentMenuItem> mangaMenu;

    public ObservableCollection<ContentMenuItem> MangaMenu
    {
      get { return mangaMenu; }
      set
      {
        mangaMenu = value;
        OnPropertyChanged();
      }
    }

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

    public MangaModel(IManga manga, LibraryViewModel model)
    {
      this.MangaMenu = new ObservableCollection<ContentMenuItem>
      {
        new OpenFolderCommand(model),
        new ChangeUpdateMangaCommand(manga?.NeedUpdate ?? false, model),
        new UpdateMangaCommand(model),
        new CompressMangaCommand(model),
        new OpenUrlMangaCommand(model),
        new HistoryClearMangaCommand(model),
        new DeleteMangaCommand(model),
        new ShowPropertiesMangaCommand(model)
      };
      this.MangaMenu.First().IsDefault = true;
      UpdateProperties(manga);
    }
  }
}