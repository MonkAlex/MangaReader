using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using MangaReader.Manga;

namespace MangaReader.ViewModel.Manga
{
  public class MangaViewModel : MangaBaseModel, IComparer<Mangas>, IComparer
  {
    private string name;
    private string type;
    private string completedIcon;
    private string needUpdateIcon;
    private double downloaded;
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
        this.Downloaded = Manga.Downloaded;
      if (args.PropertyName == nameof(Manga.IsCompleted))
        SetCompletedIcon(Manga.IsCompleted);
      if (args.PropertyName == nameof(Manga.Uri))
        SetType(Manga.Uri);
      if (args.PropertyName == nameof(Manga.NeedUpdate))
        SetNeedUpdate(Manga.NeedUpdate);
      if (args.PropertyName == nameof(Manga.Status))
        this.Status = Manga.Status;
    }

    private void SetCompletedIcon(bool isCompleted)
    {
      var result = "Icons/play.png";
      switch (isCompleted)
      {
        case true:
          result = "Icons/stop.png";
          break;

        case false:
          result = "Icons/play.png";
          break;
      }
      this.CompletedIcon = result;
    }

    private void SetType(Uri uri)
    {
      var s = uri == null ? string.Empty : uri.ToString();
      var result = "NA";
      if (s.Contains("readmanga"))
        result = "RM";
      if (s.Contains("adultmanga"))
        result = "AM";
      if (s.Contains("acomics"))
        result = "AC";
      if (s.Contains("hentaichan"))
        result = "HC";
      if (s.Contains("mintmanga.com"))
        result = "MM";
      this.Type = result;
    }

    private void SetNeedUpdate(bool needUpdate)
    {
      var result = "Icons/play.png";
      switch (needUpdate)
      {
        case true:
          result = "Icons/yes.png";
          break;

        case false:
          result = "Icons/no.png";
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
      throw new Exception("Can compare only Mangas.");
    }

    public int Compare(Mangas x, Mangas y)
    {
      return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
    }

    public MangaViewModel(Mangas manga) : base(manga)
    {
      if (Manga != null)
      {
        this.Name = Manga.Name;
        this.Downloaded = Manga.Downloaded;
        SetCompletedIcon(Manga.IsCompleted);
        SetType(Manga.Uri);
        SetNeedUpdate(Manga.NeedUpdate);
        this.Status = Manga.Status;
        Manga.PropertyChanged += MangaOnPropertyChanged;
      }
    }
  }
}