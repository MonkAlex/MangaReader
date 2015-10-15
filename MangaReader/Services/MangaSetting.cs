using System;
using MangaReader.Account;

namespace MangaReader.Services
{
  public class MangaSetting : Entity.Entity
  {
    public virtual Guid Manga { get; set; }

    public virtual string MangaName { get; set; }

    public virtual string Folder { get; set; }

    /// <summary>
    /// Сжимать скачанную мангу.
    /// </summary>
    public virtual bool CompressManga { get; set; }

    /// <summary>
    /// Обновлять при скачивании (true) или скачивать целиком(false).
    /// </summary>
    public virtual bool OnlyUpdate { get; set; }

    public virtual Login Login
    {
      get { return login ?? (login = Login.Create(this.Manga)); }
      set { login = value; }
    }

    private Login login;

    public virtual Compression.CompressionMode DefaultCompression { get; set; }

    public MangaSetting()
    {
      this.CompressManga = true;
      this.OnlyUpdate = true;
    }
  }
}
