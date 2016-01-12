using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MangaReader.Services.Config
{
  public class LibraryFilter
  {
    public static Dictionary<string, object> AllowedTypes
    {
      get
      {
        if (_allowedTypes == null)
        {
          _allowedTypes = new Dictionary<string, object>(AllTypes);
        }
        return _allowedTypes;
      }
      set { _allowedTypes = value; }
    }

    public static Dictionary<string, object> AllTypes
    {
      get
      {
        if (_allTypes == null)
        {
          _allTypes = ConfigStorage.Instance.DatabaseConfig.MangaSettings
            .Select(s => new { s.MangaName, s })
            .OrderBy(a => a.MangaName)
            .ToDictionary(a => a.MangaName, a => a.s as object);
        }
        return _allTypes;
      }
    }

    private static Dictionary<string, object> _allTypes;

    private static Dictionary<string, object> _allowedTypes;

    public bool Uncompleted { get; set; }

    public bool OnlyUpdate { get; set; }

    public string Name { get; set; }

    public SortDescription SortDescription { get; set; }

    public LibraryFilter()
    {
      Name = string.Empty;
    }
  }
}