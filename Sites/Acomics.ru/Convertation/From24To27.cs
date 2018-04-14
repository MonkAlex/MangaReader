using System;
using System.Linq;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;

namespace Acomics.Convertation
{
  public class From24To27 : MangasConverter
  {
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && this.CanConvertVersion(process);
    }

    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      var parser = new Parser();
      using (var context = Repository.GetEntityContext())
      {
        var acomics = context.Get<Acomics>().ToList();
        foreach (var acomic in acomics)
        {
          parser.UpdateContentType(acomic);
          process.Percent += 100.0 / acomics.Count;
        }
        acomics.SaveAll();
      }
    }

    public From24To27()
    {
      this.Version = new Version(1, 27, 5584);
      this.CanReportProcess = true;
    }
  }
}