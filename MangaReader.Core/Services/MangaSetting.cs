using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

    /// <summary>
    /// Идентификатор выбранной стратегии именования папок.
    /// </summary>
    public Guid FolderNamingStrategy { get; set; }

    public override async Task BeforeSave(ChangeTrackerArgs args)
    {
      if (!args.IsNewEntity)
      {
        var folderState = args.GetPropertyState<string>(nameof(Folder));
        var uriState = args.GetPropertyState<Uri>(nameof(MainUri));
        if (folderState.IsChanged || uriState.IsChanged)
        {
          using (var context = Repository.GetEntityContext())
          {
            if (uriState.IsChanged && 
                Login != null && 
                Equals(Login.MainUri, uriState.OldValue) &&
                !Equals(Login.MainUri, uriState.Value))
              Login.MainUri = MainUri;

            var mangas = context.Get<IManga>().Where(m => m.Setting == this).ToList();
            foreach (var manga in mangas)
            {
              if (uriState.IsChanged)
                manga.Uri = new Uri(manga.Uri.OriginalString.Replace(uriState.OldValue.GetLeftPart(UriPartial.Authority), uriState.Value.GetLeftPart(UriPartial.Authority)));
              if (folderState.IsChanged)
                manga.RefreshFolder();
              await context.AddToTransaction(manga).ConfigureAwait(false);
            }
          }
        }
      }

      await base.BeforeSave(args).ConfigureAwait(false);
    }

    public MangaSetting()
    {
      this.CompressManga = true;
      this.OnlyUpdate = true;
    }
  }
}
