using System.Diagnostics;
using System.IO;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class OpenFolderCommand : MangaBaseCommand
  {
    public override void Execute(Mangas manga)
    {
      if (manga != null && Directory.Exists(manga.Folder))
        Process.Start(manga.Folder);
      else
        Library.Status = Strings.Library_Status_FolderNotFound;
    }

    public override bool CanExecute(object parameter)
    {
      return true;
    }

    public OpenFolderCommand() : base(null)
    {
      this.Name = Strings.Manga_Action_OpenFolder;
      this.NeedRefresh = false;
    }
  }
}