using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MangaReader.Core.Exception;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.ViewModel.Manga
{
  public class MangaViewModel : MangaBaseModel, IComparer<IManga>, IComparer
  {
    private string name;
    private string type;
    private string completedIcon;
    private string needUpdateIcon;
    private double downloaded;
    private string speed;
    private string status;
    private bool speedTracked = false;

    public string Type
    {
      get { return type; }
      set
      {
        type = value;
        OnPropertyChanged();
      }
    }

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

    public string Name
    {
      get { return name; }
      set
      {
        name = value;
        OnPropertyChanged();
      }
    }

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

    private void MangaOnPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
      if (args.PropertyName == nameof(Manga.Name))
        this.Name = Manga.Name;
      if (args.PropertyName == nameof(Manga.Downloaded))
      {
        if (!speedTracked && !Manga.IsDownloaded)
        {
          NetworkSpeed.Updated += NetworkSpeedOnUpdated;
          speedTracked = true;
        }
        this.Downloaded = Manga.Downloaded;
      }
      if (args.PropertyName == nameof(Manga.IsCompleted))
        SetCompletedIcon(Manga.IsCompleted);
      if (args.PropertyName == nameof(Manga.Uri))
        SetType(Manga);
      if (args.PropertyName == nameof(Manga.NeedUpdate))
        SetNeedUpdate(Manga.NeedUpdate);
      if (args.PropertyName == nameof(Manga.Status))
        this.Status = Manga.Status;
      if (args.PropertyName == nameof(Manga.IsDownloaded))
      {
        if (Manga.IsDownloaded && speedTracked)
        {
          NetworkSpeed.Updated -= NetworkSpeedOnUpdated;
          speedTracked = false;
        }
      }
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
      this.NeedUpdateIcon = result;
    }

    public int Compare(object x, object y)
    {
      var xM = x as MangaViewModel;
      var yM = y as MangaViewModel;
      if (xM != null && yM != null)
        return Compare(xM.Manga, yM.Manga);
      throw new MangaReaderException("Can compare only Mangas.");
    }

    public int Compare(IManga x, IManga y)
    {
      return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
    }

    public MangaViewModel(IManga manga, LibraryViewModel model) : base(manga, model)
    {
      if (Manga != null)
      {
        this.Name = Manga.Name;
        this.Downloaded = Manga.Downloaded;
        SetCompletedIcon(Manga.IsCompleted);
        SetType(Manga);
        SetNeedUpdate(Manga.NeedUpdate);
        this.Status = Manga.Status;
        if (Manga is INotifyPropertyChanged)
          ((INotifyPropertyChanged)Manga).PropertyChanged += MangaOnPropertyChanged;
      }
    }

    private void NetworkSpeedOnUpdated(double d)
    {
      var outSpeed = NetworkSpeed.TotalSpeed;
      this.Speed = (!Manga.IsDownloaded && outSpeed != 0) ? (outSpeed.HumanizeByteSize() + "ps") : string.Empty;
    }
  }
}