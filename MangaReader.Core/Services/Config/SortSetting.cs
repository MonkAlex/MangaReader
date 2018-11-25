using System.Collections.Generic;
using System.ComponentModel;
using MangaReader.Core.Manga;

namespace MangaReader.Core.Services.Config
{
  public class SortSetting
  {
    static SortSetting()
    {
      Sorts = new List<SortSetting>()
      {
        new SortSetting("По имени", new SortDescription(nameof(IManga.Name), ListSortDirection.Ascending)),
        new SortSetting("Последние добавленные", new SortDescription(nameof(IManga.Created), ListSortDirection.Descending)),
        new SortSetting("Последние обновлённые", new SortDescription(nameof(IManga.DownloadedAt), ListSortDirection.Descending))
      };
    }

    public static readonly IReadOnlyList<SortSetting> Sorts;

    public SortDescription SortDescription { get; set; }

    public string Name { get; set; }

    public override string ToString()
    {
      return Name;
    }

    public SortSetting(string name, SortDescription description)
    {
      this.Name = name;
      this.SortDescription = description;
    }
  }
}