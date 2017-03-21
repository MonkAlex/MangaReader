using System;
using System.IO;
using MangaReader.Core.Convertation.Primitives;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Core.Convertation.Database
{
  public class From40To41 : DatabaseConverter
  {
    protected override void ProtectedConvert(IProcess process)
    {
      base.ProtectedConvert(process);

      foreach (var setting in ConfigStorage.Instance.DatabaseConfig.MangaSettings)
      {
        Uri folderUri;
        if (!Uri.TryCreate(setting.Folder, UriKind.RelativeOrAbsolute, out folderUri))
          continue;

        if (folderUri.IsAbsoluteUri)
        {
          var relativePath = DirectoryHelpers.GetRelativePath(ConfigStorage.WorkFolder, setting.Folder);
          setting.Folder = relativePath.StartsWith(@"..\..\") ? setting.Folder : relativePath;
        }
        
      }
      
    }

    public From40To41()
    {
      this.Version = new Version(1, 40, 5);
    }
  }
}