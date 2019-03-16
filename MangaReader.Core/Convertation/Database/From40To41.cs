using System;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using System.Linq;
using System.Threading.Tasks;
using MangaReader.Core.NHibernate;

namespace MangaReader.Core.Convertation.Database
{
  public class From40To41 : DatabaseConverter
  {
    protected override async Task ProtectedConvert(IProcess process)
    {
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
          await context.Save(setting).ConfigureAwait(false);
        }
      }
    }

    public From40To41()
    {
      this.Version = new Version(1, 40, 5);
    }
  }
}