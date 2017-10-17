using System;
using System.Collections.Generic;

namespace MangaReader.Core.DataTrasferObject
{
  public class VolumeDto
  {
    public ICollection<ChapterDto> Container { get; set; }
    public int Number { get; set; }
    public string Name { get; set; }
    public Uri Uri { get; set; }

    public VolumeDto()
    {
      this.Container = new List<ChapterDto>();
    }

    public VolumeDto(int number) : this()
    {
      this.Number = number;
    }

  }
}