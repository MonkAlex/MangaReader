using System;
using System.Collections.Generic;
using MangaReader.Core.Account;

namespace MangaReader.Core.Services
{
  public class MangaSetting : Entity.Entity
  {
    public Guid Manga { get; set; }

    public string MangaName { get; set; }

    public string Folder { get; set; }

    /// <summary>
    /// Сжимать скачанную мангу.
    /// </summary>
    public bool CompressManga { get; set; }

    /// <summary>
    /// Обновлять при скачивании (true) или скачивать целиком(false).
    /// </summary>
    public bool OnlyUpdate { get; set; }

    public virtual ILogin Login { get; set; }

    public virtual Compression.CompressionMode DefaultCompression { get; set; }

    public Uri MainUri { get; set; }

    public IList<Uri> MangaSettingUris { get; set; }

    /// <summary>
    /// Идентификатор выбранной стратегии именования папок.
    /// </summary>
    public Guid FolderNamingStrategy { get; set; }

    public MangaSetting()
    {
      this.CompressManga = true;
      this.OnlyUpdate = true;
      this.MangaSettingUris = new List<Uri>();
    }
  }
}
