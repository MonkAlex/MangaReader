using System;
using System.Collections;
using System.Collections.Generic;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;

namespace MangaReader.Core
{
  public abstract class BaseSiteParser : ISiteParser
  {
    public abstract void UpdateNameAndStatus(IManga manga);

    public virtual void UpdateContentType(IManga manga)
    {
      // Content type cannot be changed.
    }

    public abstract void UpdateContent(IManga manga);

    public abstract UriParseResult ParseUri(Uri uri);

    protected void UpdateName(IManga manga, string newName)
    {
      if (string.IsNullOrWhiteSpace(newName))
        Log.AddFormat("Не удалось получить имя манги, текущее название - '{0}'.", manga.ServerName);
      else if (newName != manga.ServerName)
        manga.ServerName = newName;
    }

    public abstract IEnumerable<byte[]> GetPreviews(IManga manga);

    public abstract IEnumerable<IManga> Search(string name);
  }
}