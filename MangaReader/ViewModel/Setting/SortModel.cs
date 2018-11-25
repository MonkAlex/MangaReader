using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.ViewModel.Primitive;
using SortDescription = MangaReader.Core.Services.Config.SortDescription;

namespace MangaReader.ViewModel.Setting
{
  public class SortModel : BaseViewModel
  {
    private SortSetting selected;
    public IReadOnlyList<SortSetting> Sorts { get; }

    public SortSetting Selected
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
      this.Sorts = SortSetting.Sorts;
      this.Selected = Sorts.FirstOrDefault();
    }
  }
}