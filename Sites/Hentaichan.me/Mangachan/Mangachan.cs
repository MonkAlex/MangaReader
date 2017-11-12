using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

    public override void Refresh()
    {
      if (Parser.ParseUri(Uri).Kind != UriParseKind.Manga)
      {
        var page = Page.GetPage(Uri);
        if (page.HasContent)
        {
          var match = Regex.Match(page.Content, "content_id\":\"(.*?)\"", RegexOptions.IgnoreCase);
          if (match.Groups.Count > 1)
          {
            Uri = new Uri(Uri, match.Groups[1].Value);
          }
        }
      }

      base.Refresh();
    }

    protected override void Created(Uri url)
    {
      base.Created(url);

      if (this.Uri != url && Parser.ParseUri(url).Kind != UriParseKind.Manga)
      {
        this.UpdateContent();
        
        var chapters = this.Volumes.SelectMany(v => v.Container);
        AddHistoryReadedUris(chapters, url);
      }

    }
  }
}
