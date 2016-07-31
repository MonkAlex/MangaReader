using System;
using System.Linq;
using MangaReader.Core.Convertation;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Services.Config;

namespace Grouple.Convertation
{
  public class From37To38 : ConfigConverter
  {
    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      var settings = ConfigStorage.Instance.DatabaseConfig.MangaSettings;
      var readmanga = settings.SingleOrDefault(s => Readmanga.Type == s.Manga);
      var mintmanga = settings.SingleOrDefault(s => Mintmanga.Type == s.Manga);
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
          mintmanga.Save();
        }
      }

      var setting = ConfigStorage.GetPlugin<Readmanga>().GetSettings();
      if (setting != null && setting.MainUri == null)
      {
        setting.MainUri = new Uri("http://readmanga.me/");
        setting.MangaSettingUris.Add(setting.MainUri);
        setting.MangaSettingUris.Add(new Uri("http://readmanga.ru/"));
        setting.Login.MainUri = new Uri(@"http://grouple.ru/");
        setting.Save();
      }

      setting = ConfigStorage.GetPlugin<Mintmanga>().GetSettings();
      if (setting != null && setting.MainUri == null)
      {
        setting.MainUri = new Uri("http://mintmanga.com/");
        setting.MangaSettingUris.Add(setting.MainUri);
        setting.MangaSettingUris.Add(new Uri("http://adultmanga.ru/"));
        setting.Login.MainUri = new Uri(@"http://grouple.ru/");
        setting.Save();
      }
    }

    public From37To38()
    {
      this.Version = new Version(1, 37, 3);
    }
  }
}