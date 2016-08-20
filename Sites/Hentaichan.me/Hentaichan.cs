using System;
using System.Collections.Generic;
using System.Linq;
using MangaReader.Core.Manga;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;

namespace Hentaichan
{
  public class Hentaichan : Mangas
  {
    public override List<Compression.CompressionMode> AllowedCompressionModes
    {
      get { return base.AllowedCompressionModes.Where(m => !Equals(m, Compression.CompressionMode.Chapter)).ToList(); }
    }

    public override bool HasVolumes { get { return false; } }

    public override bool HasChapters { get { return true; } }

    public override void Refresh()
    {
      base.Refresh();

      var newName = Getter.GetMangaName(this.Uri);
      if (string.IsNullOrWhiteSpace(newName))
        Log.AddFormat("Не удалось получить имя манги, текущее название - '{0}'.", this.ServerName);
      else if (newName != this.ServerName)
        this.ServerName = newName;
    }

    public override bool IsValid()
    {
      var baseValidation = base.IsValid();
      if (!baseValidation)
        return false;

      var doubles = Repository.Get<Hentaichan>().Any(m => m.Id != this.Id && m.ServerName == this.ServerName);

      return !doubles;
    }

    protected override void UpdateContent()
    {
      this.Pages.Clear();
      this.Chapters.Clear();
      this.Volumes.Clear();

      Getter.UpdateContent(this);

      base.UpdateContent();
    }
  }
}
