﻿using System.Diagnostics;
using System.IO;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class OpenFolderCommand : MangaBaseCommand
  {
    public override void Execute(IManga manga)
    {
      this.Execute(manga as IDownloadable);
    }

    public void Execute(IDownloadable parameter)
    {
      if (parameter != null && Directory.Exists(parameter.GetAbsoulteFolderPath()))
        Process.Start(parameter.GetAbsoulteFolderPath());
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
      this.Icon = "pack://application:,,,/Icons/Manga/open_folder.png";
      this.NeedRefresh = false;
    }
  }
}