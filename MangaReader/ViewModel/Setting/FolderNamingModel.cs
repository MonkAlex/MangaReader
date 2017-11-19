using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Services;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel.Setting
{
  public class FolderNamingModel : BaseViewModel
  {
    private FolderNamingStrategyDto selected;
    public List<FolderNamingStrategyDto> Strategies { get; }

    public FolderNamingStrategyDto Selected
    {
      get { return selected; }
      set
      {
        selected = value;
        OnPropertyChanged();
      }
    }

    public Guid SelectedGuid
    {
      get { return Selected.Id; }
      set { Selected = Strategies.Single(s => s.Id == value); }
    }

    public FolderNamingModel()
    {
      this.Strategies = FolderNamingStrategies.Strategies.Select(s => new FolderNamingStrategyDto(s)).ToList();
    }
  }
}