using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace MangaReader.Core.Services.Config
{
  public class LibraryFilter : INotifyPropertyChanged
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

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}