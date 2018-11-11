using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.ViewModel.Primitive;
using SortDescription = MangaReader.Core.Services.Config.SortDescription;

namespace MangaReader.ViewModel.Setting
{
  public class SortModel : BaseViewModel
  {
    private SortDto selected;
    public List<SortDto> Sorts { get; }

    public SortDto Selected
    {
      get { return selected; }
      set
      {
        selected = value;
        OnPropertyChanged();
      }
    }

    public SortDescription SelectedDescription
    {
      get { return Selected.SortDescription; }
      set { Selected = Sorts.Single(s => s.SortDescription == value); }
    }

    public SortModel()
    {
      this.Sorts = new List<SortDto>()
      {
        new SortDto("По имени", new SortDescription(nameof(IManga.Name), ListSortDirection.Ascending)),
        new SortDto("Последние добавленные", new SortDescription(nameof(IManga.Created), ListSortDirection.Descending)),
        new SortDto("Последние обновлённые", new SortDescription(nameof(IManga.DownloadedAt), ListSortDirection.Descending))
      };
      this.Selected = Sorts.FirstOrDefault();
    }
  }
}