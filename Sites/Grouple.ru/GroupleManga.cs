using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MangaReader.Core;
using MangaReader.Core.Manga;
using MangaReader.Core.Properties;
using MangaReader.Core.Services;

namespace Grouple
{
  /// <summary>
  /// Манга.
  /// </summary>
  public abstract class GroupleManga : Mangas
  {
    #region Свойства

    /// <summary>
    /// Статус перевода.
    /// </summary>
    public override bool IsCompleted
    {
      get
      {
        if (this.Status == null)
          return false;
        var match = Regex.Match(this.Status, Strings.Manga_IsCompleted);
        return match.Groups.Count > 1 && match.Groups[1].Value.Trim() == "завершен";
      }
    }

    public override List<Compression.CompressionMode> AllowedCompressionModes
    {
      get { return base.AllowedCompressionModes.Where(m => !Equals(m, Compression.CompressionMode.Manga)).ToList(); }
    }

    /// <summary>
    /// Признак наличия глав.
    /// </summary>
    public override bool HasChapters { get { return true; } }

    /// <summary>
    /// Признак наличия томов.
    /// </summary>
    public override bool HasVolumes { get { return true; } }

    #endregion

    #region Методы

    /// <summary>
    /// Обновить информацию о манге - название, главы, обложка.
    /// </summary>
    public override async Task Refresh()
    {
      var page = await Page.GetPageAsync(this.Uri).ConfigureAwait(false);
      if (!page.HasContent)
        return;

      // Если сайт ответил с другого адреса - переписываем текущий адрес.
      if (page.ResponseUri != this.Uri)
      {
        this.Uri = page.ResponseUri;
        await this.Refresh().ConfigureAwait(false);
        return;
      }

      // Если на странице редирект - выполняем его и получаем новую ссылку на мангу.
      if (page.Content.ToLowerInvariant().Contains(Grouple.Parser.CookieKey))
      {
        var newUri = await Grouple.Parser.GetRedirectUri(page).ConfigureAwait(false);
        if (!this.Uri.Equals(newUri))
        {
          this.Uri = newUri;
          await this.Refresh().ConfigureAwait(false);
          return;
        }
      }

      await base.Refresh().ConfigureAwait(false);
    }

    protected override async Task CreatedFromWeb(Uri url)
    {
      if (this.Uri != url && Parser.ParseUri(url).Kind != UriParseKind.Manga)
      {
        await this.UpdateContent().ConfigureAwait(false);

        var chapters = this.Volumes.SelectMany(v => v.Container);
        AddHistoryReadedUris(chapters, url);
      }

      await base.CreatedFromWeb(url).ConfigureAwait(false);
    }

    #endregion
  }
}
