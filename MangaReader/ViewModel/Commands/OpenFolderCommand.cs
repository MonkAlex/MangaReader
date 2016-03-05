using System.Diagnostics;
using System.IO;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class OpenFolderCommand : BaseCommand
  {
    public override string Name { get { return Strings.Manga_Action_OpenFolder; } }

    public override void Execute(object parameter)
    {
      var manga = parameter as IDownloadable;
      if (manga != null && Directory.Exists(manga.Folder))
        Process.Start(manga.Folder);
      else
        Library.Status = Strings.Library_Status_FolderNotFound;
    }
  }
}