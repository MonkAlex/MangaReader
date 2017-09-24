using System;
using System.Linq;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Acomics.Convertation
{
  public class From24To27 : MangasConverter
  {
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && 
        Version.CompareTo(MangaReader.Core.NHibernate.Repository.Get<DatabaseConfig>().Single().Version) > 0 && 
        process.Version.CompareTo(Version) >= 0;
    }

    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      var acomics = Repository.Get<Acomics>().ToList();
      var parser = new Parser();
      foreach (var acomic in acomics)
      {
        parser.UpdateContentType(acomic);
        process.Percent += 100.0 / acomics.Count;
      }
      acomics.SaveAll();
    }

    public From24To27()
    {
      this.Version = new Version(1, 27, 5584);
      this.CanReportProcess = true;
    }
  }
}