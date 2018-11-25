using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using MangaReader.Core.Exception;
using MangaReader.Core.Manga;
using Newtonsoft.Json;

namespace MangaReader.Core.Services.Config
{
  public class LibraryFilter : INotifyPropertyChanged, IComparer, IComparer<IManga>, IComparer<ILibraryFilterableItem>
  {
    [JsonIgnore]
    public Dictionary<string, object> AllowedTypes
    {
      get
      {
        if (allowedTypes == null)
        {
          allowedTypes = new Dictionary<string, object>(AllTypes);
        }
        return allowedTypes;
      }
      set
      {
        allowedTypes = value;
        OnPropertyChanged();
      }
    }

    [JsonIgnore]
    public Dictionary<string, object> AllTypes
    {
      get
      {
        if (allTypes == null)
        {
          allTypes = NHibernate.Repository.GetStateless<MangaSetting>()
            .Select(s => new { s.MangaName, s })
            .OrderBy(a => a.MangaName)
            .ToDictionary(a => a.MangaName, a => a.s.Id as object);
        }
        return allTypes;
      }
    }

    private Dictionary<string, object> allTypes;

    private Dictionary<string, object> allowedTypes;
    private bool uncompleted;
    private bool onlyUpdate;
    private string name;
    private SortDescription sortDescription;

    public bool Uncompleted
    {
      get { return uncompleted; }
      set
      {
        uncompleted = value;
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

    public string Name
    {
      get { return name; }
      set
      {
        name = value;
        OnPropertyChanged();
      }
    }

    public SortDescription SortDescription
    {
      get { return sortDescription; }
      set
      {
        sortDescription = value;
        OnPropertyChanged();
      }
    }

    public LibraryFilter()
    {
      Name = string.Empty;
    }

    public int Compare(object x, object y)
    {
      if (x is ILibraryFilterableItem xM && y is ILibraryFilterableItem yM)
      {
        return Compare(xM, yM);
      }
      if (x is IManga xZ && y is IManga yZ)
      {
        return Compare(xZ, yZ);
      }
      throw new MangaReaderException("Can compare only Mangas.");
    }

    public int Compare(ILibraryFilterableItem x, ILibraryFilterableItem y)
    {
      if (SortDescription.PropertyName == nameof(IManga.Created))
        return CompareByDate(x.Created, y.Created);
      if (SortDescription.PropertyName == nameof(IManga.DownloadedAt))
        return CompareByDate(x.DownloadedAt, y.DownloadedAt);
      return CompareByName(x.Name, y.Name);
    }

    private int CompareByDate(DateTime? x, DateTime? y)
    {
      if (SortDescription.Direction == ListSortDirection.Ascending)
        return DateTime.Compare(x ?? DateTime.MinValue, y ?? DateTime.MinValue);

      return DateTime.Compare(y ?? DateTime.MinValue, x ?? DateTime.MinValue);
    }

    private int CompareByName(string x, string y)
    {
      if (SortDescription.Direction == ListSortDirection.Ascending)
        return string.Compare(x, y, StringComparison.InvariantCultureIgnoreCase);

      return string.Compare(y, x, StringComparison.InvariantCultureIgnoreCase);
    }

    public int Compare(IManga x, IManga y)
    {
      if (SortDescription.PropertyName == nameof(IManga.Created))
        return CompareByDate(x.Created, y.Created);
      if (SortDescription.PropertyName == nameof(IManga.DownloadedAt))
        return CompareByDate(x.DownloadedAt, y.DownloadedAt);
      return CompareByName(x.Name, y.Name);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }

  public interface ILibraryFilterableItem
  {
    DateTime? Created { get; set; }
    DateTime? DownloadedAt { get; set; }
    string Name { get; set; }
  }
}