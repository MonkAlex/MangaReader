using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MangaReader.Avalonia.ViewModel.Command.Manga;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Avalonia.ViewModel.Explorer
{
  public class MangaModel : ExplorerTabViewModel, ILibraryFilterableItem
  {
    #region MangaProperties

    private string name;
    private string folder;
    private bool? needCompress;
    private Compression.CompressionMode? compressionMode;

    public int Id { get; set; }

    public string Uri
    {
      get => uri;
      set => RaiseAndSetIfChanged(ref uri, value);
    }

    private string uri;

    public bool Saved { get { return Id != 0; } }

    public bool IsUndoCompleted { get; private set; }

    public string MangaName
    {
      get => name;
      set => RaiseAndSetIfChanged(ref name, value);
    }

    public string OriginalName { get; protected set; }

    public string Folder
    {
      get => folder;
      set => RaiseAndSetIfChanged(ref folder, value);
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

    public string Description
    {
      get => description;
      set => RaiseAndSetIfChanged(ref description, value);
    }

    private string description;

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

    public byte[] Cover
    {
      get => cover;
      set => RaiseAndSetIfChanged(ref cover, value);
    }

    private byte[] cover;

    #endregion

    public void UpdateProperties(IManga manga)
    {
      this.Name = manga?.Name ?? "<Empty>";

      if (manga == null)
        return;

      this.Id = manga.Id;
      this.Uri = manga.Uri.OriginalString;
      this.MangaName = manga.Name;
      this.OriginalName = manga.ServerName;
      this.Folder = manga.Folder;
      this.NeedCompress = manga.NeedCompress;
      this.CompressionModes = new List<Compression.CompressionMode>(manga.AllowedCompressionModes);
      this.CompressionMode = manga.CompressionMode;
      if (manga.Downloaded > Downloaded)
        this.Downloaded = manga.Downloaded;
      SetCompletedIcon(manga.IsCompleted);
      SetType(manga);
      SetNeedUpdate(manga.NeedUpdate);
      this.Status = manga.Status;
      if (manga.Setting != null)
        this.SettingsId = manga.Setting.Id;
      this.Created = manga.Created;
      this.DownloadedAt = manga.DownloadedAt;
      this.Cover = manga.Cover;
      this.Description = manga.Description;

      this.IsUndoCompleted = false;
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

    public void RestoreName()
    {
      this.MangaName = this.OriginalName;
    }

    public void OpenHyperlink()
    {
      Helper.StartUseShell(this.Uri);
    }

    private void SetType(IManga manga)
    {
      var result = "NA";
      var plugin = ConfigStorage.Plugins?.SingleOrDefault(p => p.MangaType == manga.GetType());
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

    public override async Task OnUnselected(ExplorerTabViewModel newModel)
    {
      await this.UndoChangedImpl().ConfigureAwait(true);
      ExplorerViewModel.Instance.Tabs.Remove(this);
      await base.OnUnselected(newModel).ConfigureAwait(true);
    }

    public ICommand Save => new MangaSaveCommand(this, ExplorerViewModel.Instance.Tabs.OfType<LibraryViewModel>().First().Library);

    public ICommand OpenFolder => new OpenFolderCommand(ExplorerViewModel.Instance.Tabs.OfType<LibraryViewModel>().First());

    public async Task UndoChanged()
    {
      await UndoChangedImpl().ConfigureAwait(true);
      ExplorerViewModel.Instance.SelectedTab = ExplorerViewModel.Instance.Tabs.OfType<LibraryViewModel>().FirstOrDefault();
    }

    private async Task UndoChangedImpl()
    {
      if (Saved && !IsUndoCompleted)
      {
        using (var context = Repository.GetEntityContext($"Undo any changes in memory for manga {OriginalName}"))
        {
          var manga = await context.Get<IManga>().FirstAsync(m => m.Id == Id).ConfigureAwait(true);
          UpdateProperties(manga);
        }

        IsUndoCompleted = true;
      }
    }

    public MangaModel(IManga manga)
    {
      UpdateProperties(manga);
    }
  }
}
