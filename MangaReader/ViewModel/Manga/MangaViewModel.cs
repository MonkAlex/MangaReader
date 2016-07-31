using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.Services.Config;

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
        SetType(Manga);
      if (args.PropertyName == nameof(Manga.NeedUpdate))
        SetNeedUpdate(Manga.NeedUpdate);
      if (args.PropertyName == nameof(Manga.Status))
        this.Status = Manga.Status;
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

    private void SetType(Mangas manga)
    {
      var result = "NA";
#warning Ещё косяк, который должен уйти в плагины
      /*
      if (manga is Core.Manga.Grouple.Readmanga)
        result = "RM";
      if (manga is Core.Manga.Acomic.Acomics)
        result = "AC";
      if (manga is Core.Manga.Hentaichan.Hentaichan)
        result = "HC";
      if (manga is Core.Manga.Grouple.Mintmanga)
        result = "MM";
        */
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
        SetType(Manga);
        SetNeedUpdate(Manga.NeedUpdate);
        this.Status = Manga.Status;
        Manga.PropertyChanged += MangaOnPropertyChanged;
      }
    }
  }
}