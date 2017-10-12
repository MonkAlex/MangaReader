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
  }

  public class ChapterDto
  {
    public ICollection<MangaPageDto> Container { get; set; }
    public double Number { get; set; }
    public string Name { get; set; }
    public Uri Uri { get; set; }

    public ChapterDto(string uri, string name, Func<string, double> getNumber) : this(uri, name)
    {
      this.Number = getNumber(uri);
    }

    public ChapterDto(string uri, string name, Func<Uri, double> getNumber) : this(uri, name)
    {
      this.Number = getNumber(Uri);
    }

    public ChapterDto(string uri, string name) : this()
    {
      this.Uri = new Uri(uri);
      this.Name = name;
    }

    public ChapterDto()
    {
      this.Container = new List<MangaPageDto>();
    }
  }

  public class MangaPageDto
  {
    public int Number { get; set; }
    public string Name { get; set; }
    public Uri Uri { get; set; }
    public Uri ImageLink { get; set; }

    public MangaPageDto(Uri uri, Uri image, int number, string desc) : this(uri, image, number)
    {
      this.Name = desc;
    }

    public MangaPageDto(Uri uri, Uri image, int number)
    {
      this.Uri = uri;
      this.ImageLink = image;
      this.Number = number;
    }
  }
}