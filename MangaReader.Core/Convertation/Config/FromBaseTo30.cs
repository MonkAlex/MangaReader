using System;
using System.IO;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Services;
using System.Linq;
using MangaReader.Core.NHibernate;

namespace MangaReader.Core.Convertation.Config
{
  public class FromBaseTo30 : ConfigConverter
  {
    protected override bool ProtectedCanConvert(IProcess process)
    {
      return base.ProtectedCanConvert(process) && File.Exists(SettingsOldPath);
    }

    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      var settings = Serializer<object[]>.Load(SettingsOldPath);
      if (settings == null)
        return;

      try
      {
        using (var context = Repository.GetEntityContext())
        {
          var mangaSettings = context.Get<MangaSetting>().ToList();
          if (settings[1] is bool)
            mangaSettings.ForEach(ms => ms.OnlyUpdate = (bool)settings[1]);
          if (settings[4] is bool)
            mangaSettings.ForEach(ms => ms.CompressManga = (bool)settings[4]);
          mangaSettings.SaveAll(context);
        }
      }
      catch (System.Exception ex)
      {
        Log.Exception(ex);
      }
    }

    public FromBaseTo30()
    {
      this.Version = new Version(1, 30, 5765);
    }
  }
}