using System;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.NHibernate;
using MangaReader.Core.Services.Config;

namespace Grouple.Convertation
{
  public class From37To38 : ConfigConverter
  {
    protected override Task ProtectedConvert(IProcess process)
    {
      using (var context = Repository.GetEntityContext())
      {
        var settings = context.Get<MangaReader.Core.Services.MangaSetting>().ToList();
        var readmanga = settings.SingleOrDefault(s => ReadmangaPlugin.Manga == s.Manga);
        var mintmanga = settings.SingleOrDefault(s => MintmangaPlugin.Manga == s.Manga);
        if (readmanga != null)
        {
          if (mintmanga == null)
            MangaReader.Core.Services.Log.Add("Не найдены настройки для конвертации нового типа mintmanga.");
          else
          {
            mintmanga.Folder = readmanga.Folder;
            mintmanga.CompressManga = readmanga.CompressManga;
            mintmanga.OnlyUpdate = readmanga.OnlyUpdate;
            mintmanga.DefaultCompression = readmanga.DefaultCompression;
            context.Save(mintmanga);
          }
        }

        var setting = ConfigStorage.GetPlugin<Readmanga>().GetSettings();
        if (setting != null && setting.MainUri == null)
        {
          setting.MainUri = new Uri("http://readmanga.me/");
          setting.Login.MainUri = new Uri(@"https://grouple.ru/");
          context.Save(setting);
        }

        setting = ConfigStorage.GetPlugin<Mintmanga>().GetSettings();
        if (setting != null && setting.MainUri == null)
        {
          setting.MainUri = new Uri("http://mintmanga.com/");
          setting.Login.MainUri = new Uri(@"https://grouple.ru/");
          context.Save(setting);
        }
      }

      return Task.CompletedTask;
    }

    public From37To38()
    {
      this.Version = new Version(1, 37, 3);
    }
  }
}