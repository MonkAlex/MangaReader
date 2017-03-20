using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Commands.Manga;

namespace MangaReader.ViewModel.Manga
{
  public class MangaCardModel : MangaBaseModel
  {
    private string name;
    private string folder;
    private bool canChangeName;
    private bool nameIsReadonly;
    private bool? needCompress;
    private Compression.CompressionMode? compressionMode;

    public string Name
    {
      get { return name; }
      set
      {
        name = value;
        UpdateFolder();
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

    public bool CanChangeName
    {
      get { return canChangeName; }
      set
      {
        canChangeName = value;
        this.NameIsReadonly = !value;
        UpdateFolder();
        if (!value)
          this.Name = Manga.ServerName;
        OnPropertyChanged();
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

    public List<Compression.CompressionMode> CompressionModes { get; private set; }

    public Compression.CompressionMode? CompressionMode
    {
      get { return compressionMode; }
      set
      {
        compressionMode = value;
        OnPropertyChanged();
      }
    }

    private void UpdateFolder()
    {
      var mangaFolder = DirectoryHelpers.MakeValidPath(this.Name.Replace(Path.DirectorySeparatorChar, '.'));
      this.Folder = DirectoryHelpers.MakeValidPath(Path.Combine(Manga.Setting.Folder, mangaFolder));
    }

    public ICommand Save { get; private set; }

    public override void Show()
    {
      base.Show();

      var window = ViewService.Instance.TryGet<System.Windows.Window>(this);
      if (window != null)
      {
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

    public MangaCardModel(IManga manga) : base(manga)
    {
      if (Manga != null)
      {
        this.Name = Manga.Name;
        this.Folder = Manga.Folder;
        this.CanChangeName = Manga.IsNameChanged;
        this.NeedCompress = Manga.NeedCompress;
        this.CompressionModes = new List<Compression.CompressionMode>(Manga.AllowedCompressionModes);
        this.CompressionMode = Manga.CompressionMode;
        if (Manga is INotifyPropertyChanged)
          ((INotifyPropertyChanged)Manga).PropertyChanged += MangaOnPropertyChanged;
        this.Save = new MangaSaveCommand(this);
      }
    }

    private void MangaOnPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
      if (args.PropertyName == nameof(Manga.Name))
        this.Name = Manga.Name;
      if (args.PropertyName == nameof(Manga.Folder))
        this.Folder = Manga.Folder;
      if (args.PropertyName == nameof(Manga.IsNameChanged))
        this.CanChangeName = Manga.IsNameChanged;
      if (args.PropertyName == nameof(Manga.NeedCompress))
        this.NeedCompress = Manga.NeedCompress;
      if (args.PropertyName == nameof(Manga.CompressionMode))
        this.CompressionMode = Manga.CompressionMode;
    }
  }
}