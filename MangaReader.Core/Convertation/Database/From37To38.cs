using System;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Manga.Acomic;
using MangaReader.Core.Manga.Grouple;
using MangaReader.Core.Manga.Hentaichan;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Convertation.Database
{
  public class From37To38 : DatabaseConverter
  {
    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      RunSql(@"update Mangas
               set Type = '64ac91ef-bdb3-4086-be17-bb1dbe7a7656'
               where Uri like '%mintmanga.com%' or Uri like '%adultmanga.ru%'");

      // Use 'select hex(Manga), MangaName from MangaSetting' to see blob code.
      var readmanga = RunSql(@"select Folder, CompressManga, OnlyUpdate, DefaultCompression 
                               from MangaSetting 
                               where hex(Manga) = 'F4BB982C46DBC447AB0EF207E283142D'") as object[];
      if (readmanga != null)
      {
        RunSql(string.Format(@"update MangaSetting
                 set Folder = '{0}', CompressManga = '{1}', OnlyUpdate = '{2}', DefaultCompression = '{3}'
                 where hex(Manga) = '{4}'", readmanga[0], (bool)readmanga[1] ? 1 : 0, (bool)readmanga[2] ? 1 : 0, 
                 readmanga[3], "EF91AC64B3BD8640BE17BB1DBE7A7656"));
        ConfigStorage.Instance.DatabaseConfig.MangaSettings.Update();
      }

      foreach (var setting in ConfigStorage.Instance.DatabaseConfig.MangaSettings)
      {
        if (setting.MainUri != null)
          continue;

        if (setting.Manga == Hentaichan.Type)
        {
          setting.MainUri = new Uri("http://hentaichan.me/");
          setting.MangaSettingUris.Add(setting.MainUri);
          setting.MangaSettingUris.Add(new Uri("http://hentaichan.ru/"));
        }
        if (setting.Manga == Readmanga.Type)
        {
          setting.MainUri = new Uri("http://readmanga.me/");
          setting.MangaSettingUris.Add(setting.MainUri);
          setting.MangaSettingUris.Add(new Uri("http://readmanga.ru/"));
        }
        if (setting.Manga == Mintmanga.Type)
        {
          setting.MainUri = new Uri("http://mintmanga.com/");
          setting.MangaSettingUris.Add(setting.MainUri);
          setting.MangaSettingUris.Add(new Uri("http://adultmanga.ru/"));
        }
        if (setting.Manga == Acomics.Type)
        {
          setting.MainUri = new Uri("http://acomics.ru/");
          setting.MangaSettingUris.Add(setting.MainUri);
        }
        setting.Save();
      }
    }

    public From37To38()
    {
      this.Version = new Version(1, 37, 3);
    }
  }
}