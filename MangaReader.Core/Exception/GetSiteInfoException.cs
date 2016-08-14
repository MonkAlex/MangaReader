using System;
using MangaReader.Core.Manga;

namespace MangaReader.Core.Exception
{
  public class GetSiteInfoException : EntityException
  {
    public Uri Uri { get; }

    public override string FormatMessage()
    {
      return string.Format("{0} Адрес: '{1}'.", base.FormatMessage(), Uri);
    }

    public GetSiteInfoException(string message, IManga entity) : this(message, entity.Uri, entity)
    {
    }

    public GetSiteInfoException(string message, Uri uri, Entity.IEntity entity) : base(message, entity)
    {
      this.Uri = uri;
    }
  }
}