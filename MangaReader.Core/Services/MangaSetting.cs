using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Account;
using MangaReader.Core.Entity;
using MangaReader.Core.Exception;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;

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

    public override void BeforeSave(ChangeTrackerArgs args)
    {
      if (args.PreviousState == null && Id != 0)
        throw new SaveValidationException("Настройки можно сохранять только в рамках одной сессии", this);

      var folderIndex = args.PropertyNames.ToList().IndexOf(nameof(Folder));
      if (folderIndex > -1 && args.PreviousState != null)
      {
        var previous = args.PreviousState[folderIndex] as string;
        var current = args.CurrentState[folderIndex] as string;
        if (previous != current)
        {
          using (var context = Repository.GetEntityContext())
          {
            var mangas = context.Get<IManga>().Where(m => m.Setting == this).ToList();
            foreach (var manga in mangas)
            {
              manga.RefreshFolder();
              context.SaveOrUpdate(manga);
            }
          }
        }
      }

      base.BeforeSave(args);
    }

    public MangaSetting()
    {
      this.CompressManga = true;
      this.OnlyUpdate = true;
      this.MangaSettingUris = new List<Uri>();
    }
  }
}
