using System;
using MangaReader.Account;

namespace MangaReader.Services
{
  public class MangaSetting : Entity.Entity
  {
    public virtual Guid Manga { get; set; }

    public virtual string MangaName { get; set; }

    public virtual string Folder { get; set; }

    public virtual Login Login { get; set; }
  }
}
