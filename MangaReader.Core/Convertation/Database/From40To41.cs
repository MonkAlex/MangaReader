using System;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using System.Linq;
using MangaReader.Core.NHibernate;

namespace MangaReader.Core.Convertation.Database
{
  public class From40To41 : DatabaseConverter
  {
    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      using (var context = Repository.GetEntityContext())
      {
        foreach (var setting in context.Get<MangaSetting>())
        {
          if (!Uri.TryCreate(setting.Folder, UriKind.RelativeOrAbsolute, out Uri folderUri))
            continue;

          if (folderUri.IsAbsoluteUri)
          {
            var relativePath = DirectoryHelpers.GetRelativePath(ConfigStorage.WorkFolder, setting.Folder);
            setting.Folder = relativePath.StartsWith(@"..\..\") ? setting.Folder : relativePath;
          }
          setting.Save();
        }
      }
    }

    public From40To41()
    {
      this.Version = new Version(1, 40, 5);
    }
  }
}