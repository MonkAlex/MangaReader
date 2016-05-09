using System.Diagnostics;
using System.IO;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class OpenFolderCommand : MangaBaseCommand
  {
    public override void Execute(Mangas manga)
    {
      this.Execute(manga);
    }

    public void Execute(IDownloadable parameter)
    {
      if (parameter != null && Directory.Exists(parameter.Folder))
        Process.Start(parameter.Folder);
      else
        Library.Status = Strings.Library_Status_FolderNotFound;
    }

    public override bool CanExecute(object parameter)
    {
      return true;
    }

    public OpenFolderCommand()
    {
      this.Name = Strings.Manga_Action_OpenFolder;
      this.NeedRefresh = false;
    }
  }
}