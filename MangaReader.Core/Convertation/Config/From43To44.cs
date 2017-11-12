using System;
using System.Linq;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Services;

namespace MangaReader.Core.Convertation.Config
{
  public class From43To44 : ConfigConverter
  {
    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      var settings = NHibernate.Repository.GetStateless<MangaSetting>();
      foreach (var setting in settings)
      {
        this.RunSql($"update Mangas set Setting = {setting.Id} where Setting is null and Type = \"{setting.Manga}\"");
      }
    }

    public From43To44()
    {
      this.Version = new Version(1, 43, 4);
    }
  }
}