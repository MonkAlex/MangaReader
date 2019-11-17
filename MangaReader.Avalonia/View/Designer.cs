using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using MangaReader.Avalonia.ViewModel.Explorer;
using MangaReader.Core;
using MangaReader.Core.Entity;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia.View
{
  internal class Designer
  {
    public static MangaModel MangaModel { get; set; }

    private static void Init()
    {
      var proxy = new MangaProxy();
      proxy.Id = -1;
      proxy.Uri = new Uri("https://mintmanga.com/berserk");
      proxy.ServerName = "Berserk";
      proxy.IsNameChanged = true;
      proxy.LocalName = "Берсерк";
      proxy.Name = proxy.LocalName;
      proxy.Folder = "C:\\Manga\\Берсерк";
      proxy.CompressionMode = Compression.CompressionMode.Volume;
      proxy.NeedCompress = null;
      proxy.Status = "Томов: 40 , выпуск продолжается\r\nПеревод: продолжается\r\nПросмотров: 34891310\r\nЖанры: трагедия, приключения, фэнтези, героическое фэнтези, сэйнэн, боевик …\r\nАвтор: Миура Кэнтаро\r\nГод выпуска: 1989\r\nИздательства: Hakusensha, Comix-ART\r\nЖурналы: Animal House, Young Animal\r\nПереводчики: The Berserk World, Majin Scanlation, Okami";
      proxy.Description = "Гатс, известный как Чёрный Мечник, ищет убежища от демонов, охотящихся за ним, и отмщения человеку, сделавшему из него жертву на своём алтаре. С помощью только своей титанической силы, умения и меча, Гатс должен биться против жестокого рока, пока битва с ненавистью мало-помалу лишает его человечности.\r\nБерсерк — это тёмная и погружающая в раздумья история о неистовых сражениях и безжалостном роке.\r\nby South Wind";
      MangaModel = new MangaModel(proxy);
    }

    static Designer()
    {
      Init();
    }

    private class MangaProxy : IManga
    {
      public bool IsDownloaded { get; }
      public double Downloaded { get; set; }
      public Uri Uri { get; set; }
      public string Folder { get; set; }
      public DateTime? DownloadedAt { get; set; }
      public Task Download(string folder = null)
      {
        throw new NotImplementedException();
      }

      public void ClearHistory()
      {
        throw new NotImplementedException();
      }

      public void Compress()
      {
        throw new NotImplementedException();
      }

      public bool? NeedCompress { get; set; }
      public List<Compression.CompressionMode> AllowedCompressionModes { get; } = Generic.GetEnumValues<Compression.CompressionMode>();
      public Compression.CompressionMode? CompressionMode { get; set; }
      public int Id { get; set; }
      public Task BeforeSave(ChangeTrackerArgs args)
      {
        throw new NotImplementedException();
      }

      public Task BeforeDelete(ChangeTrackerArgs args)
      {
        throw new NotImplementedException();
      }

      public event PropertyChangedEventHandler PropertyChanged;
      public string Name { get; set; }
      public string LocalName { get; set; }
      public string ServerName { get; set; }
      public bool IsNameChanged { get; set; }
      public string Status { get; set; }
      public string Description { get; set; }
      public MangaSetting Setting { get; }
      public ISiteParser Parser { get; }
      public byte[] Cover { get; set; }
      public bool HasChapters { get; set; }
      public bool HasVolumes { get; set; }
      public ICollection<Volume> Volumes { get; set; }
      public ICollection<Chapter> Chapters { get; set; }
      public ICollection<MangaPage> Pages { get; set; }
      public ICollection<MangaHistory> Histories { get; }
      public DateTime? Created { get; set; }
      public bool NeedUpdate { get; set; }
      public Task<bool> IsValid()
      {
        throw new NotImplementedException();
      }

      public bool IsCompleted { get; set; }
      public Task Refresh()
      {
        throw new NotImplementedException();
      }

      public Task UpdateContent()
      {
        throw new NotImplementedException();
      }

      public void RefreshFolder()
      {
        throw new NotImplementedException();
      }
    }
  }
}
