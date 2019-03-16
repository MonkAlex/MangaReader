using System;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services;

namespace Acomics.Convertation
{
  public class From24To27 : MangasConverter
  {
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && this.CanConvertVersion(process);
    }

    protected override async Task ProtectedConvert(IProcess process)
    {
      var parser = new Parser();
      using (var context = Repository.GetEntityContext())
      {
        var acomics = context.Get<Acomics>().ToList();
        foreach (var acomic in acomics)
        {
          await parser.UpdateContentType(acomic).ConfigureAwait(false);
          process.Percent += 100.0 / acomics.Count;
        }
        await acomics.SaveAll(context).ConfigureAwait(false);
      }
    }

    public From24To27()
    {
      this.Version = new Version(1, 27, 5584);
      this.CanReportProcess = true;
    }
  }
}