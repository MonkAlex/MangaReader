using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MangaReader.Core;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;

namespace Hentaichan.Mangachan
{
  public class Mangachan : Mangas
  {
    public override bool HasVolumes { get { return true; } }

    public override bool HasChapters { get { return true; } }

    public override bool IsValid()
    {
      var baseValidation = base.IsValid();
      if (!baseValidation)
        return false;

      using (var context = Repository.GetEntityContext())
      {
        return !context.Get<Mangachan>().Any(m => m.Id != Id && m.ServerName == ServerName);
      }
    }

    public override async Task Refresh()
    {
      if (Parser.ParseUri(Uri).Kind != UriParseKind.Manga)
      {
        var page = await Page.GetPageAsync(Uri).ConfigureAwait(false);
        if (page.HasContent)
        {
          var match = Regex.Match(page.Content, "content_id\":\"(.*?)\"", RegexOptions.IgnoreCase);
          if (match.Groups.Count > 1)
          {
            Uri = new Uri(Uri, match.Groups[1].Value);
          }
        }
      }

      await base.Refresh().ConfigureAwait(false);
    }

    protected override async Task CreatedFromWeb(Uri url)
    {
      await base.CreatedFromWeb(url).ConfigureAwait(false);

      if (this.Uri != url && Parser.ParseUri(url).Kind != UriParseKind.Manga)
      {
        await this.UpdateContent().ConfigureAwait(false);
        
        var chapters = this.Volumes.SelectMany(v => v.Container);
        AddHistoryReadedUris(chapters, url);
      }

    }
  }
}
