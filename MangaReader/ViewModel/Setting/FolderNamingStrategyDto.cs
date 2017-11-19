using System;
using MangaReader.Core.Services;

namespace MangaReader.ViewModel.Setting
{
  public class FolderNamingStrategyDto
  {
    public Guid Id { get; set; }
    public string Name { get; set; }

    public override string ToString()
    {
      return Name;
    }

    public FolderNamingStrategyDto()
    {

    }

    public FolderNamingStrategyDto(IFolderNamingStrategy strategy)
    {
      this.Id = strategy.Id;
      this.Name = strategy.Name;
    }
  }
}