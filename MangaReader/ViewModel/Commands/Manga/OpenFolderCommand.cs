using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class OpenFolderCommand : MultipleMangasBaseCommand
  {
    public override void Execute(IEnumerable<IManga> mangas)
    {
      foreach (var m in mangas)
      {
        this.Execute(m as IDownloadable);
      }
    }

    public void Execute(IDownloadable parameter)
    {
      if (parameter != null && Directory.Exists(parameter.GetAbsoulteFolderPath()))
        Process.Start(parameter.GetAbsoulteFolderPath());
      else
        Log.Info(Strings.Library_Status_FolderNotFound);
    }

    public override bool CanExecute(object parameter)
    {
      return true;
    }

    public OpenFolderCommand(LibraryViewModel model) : base(model)
    {
      this.Name = Strings.Manga_Action_OpenFolder;
      this.Icon = "pack://application:,,,/Icons/Manga/open_folder.png";
      this.NeedRefresh = false;
    }
  }
}