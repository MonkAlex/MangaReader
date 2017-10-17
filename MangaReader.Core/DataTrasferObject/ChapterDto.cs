using System;
using System.Collections.Generic;

namespace MangaReader.Core.DataTrasferObject
{
  public class ChapterDto
  {
    public ICollection<MangaPageDto> Container { get; set; }
    public double Number { get; set; }
    public string Name { get; set; }
    public Uri Uri { get; set; }

    public ChapterDto(string uri, string name) : this(new Uri(uri), name) { }

    public ChapterDto(Uri uri, string name) : this()
    {
      this.Uri = uri;
      this.Name = name;
    }

    public ChapterDto()
    {
      this.Container = new List<MangaPageDto>();
    }
  }
}