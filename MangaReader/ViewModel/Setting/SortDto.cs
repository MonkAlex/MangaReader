using MangaReader.Core.Services.Config;

namespace MangaReader.ViewModel.Setting
{
  public class SortDto
  {
    public SortDescription SortDescription { get; set; }

    public string Name { get; set; }

    public override string ToString()
    {
      return Name;
    }

    public SortDto(string name, SortDescription description)
    {
      this.Name = name;
      this.SortDescription = description;
    }
  }
}