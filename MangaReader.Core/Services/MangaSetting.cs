using System;
using System.IO;
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
      if (!DirectoryHelpers.ValidateSettingPath(this.Folder))
        throw new DirectoryNotFoundException($"Не найдена папка {this.Folder}, папка должна существовать.");

      if (!args.IsNewEntity)
      {
        using (var context = Repository.GetEntityContext())
        {
          var folderState = args.GetPropertyState<string>(nameof(Folder));
          if (folderState.IsChanged)
          {
            var mangaFolders = await context.Get<IManga>().Select(m => m.Folder).ToListAsync().ConfigureAwait(false);
            mangaFolders = mangaFolders.Select(DirectoryHelpers.GetAbsoluteFolderPath).ToList();
            var settingAbsoluteFolderPath = DirectoryHelpers.GetAbsoluteFolderPath(this.Folder);
            if (mangaFolders.Any(f => DirectoryHelpers.Equals(f, settingAbsoluteFolderPath) || DirectoryHelpers.IsSubfolder(f, settingAbsoluteFolderPath)))
              throw new MangaSettingSaveValidationException($"Папка {this.Folder} используется мангой.", this);
          }

          var uriState = args.GetPropertyState<Uri>(nameof(MainUri));
          if (folderState.IsChanged || uriState.IsChanged)
          {
            if (!args.CanAddEntities)
            {
              var debugMessage = $"Manga settings {MangaName}({Manga:D}) changed:";
              if (folderState.IsChanged)
                debugMessage += $" folder changed from '{folderState.OldValue}' to '{folderState.Value}'";
              if (uriState.IsChanged)
                debugMessage += $" uri changed from '{uriState.OldValue}' to '{uriState.Value}'";
              Log.Add(debugMessage);
            }

            // Changed manga will be added to session.
            if (args.CanAddEntities)
            {
              if (uriState.IsChanged &&
                  Login != null &&
                  Equals(Login.MainUri, uriState.OldValue) &&
                  !Equals(Login.MainUri, uriState.Value))
                Login.MainUri = MainUri;

              var mangas = await context.Get<IManga>().Where(m => m.Setting == this).ToListAsync().ConfigureAwait(false);
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
