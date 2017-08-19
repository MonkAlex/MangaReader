using System.Diagnostics;
using System.IO;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;

namespace MangaReader.Avalonia.ViewModel.Command.Manga
{
  public class OpenFolderCommand : MangaCommand
  {
    public override void Execute(IManga manga)
    {
      this.Execute(manga);
    }

    public void Execute(IDownloadable parameter)
    {
      if (parameter != null && Directory.Exists(parameter.GetAbsoulteFolderPath()))
        Process.Start(parameter.GetAbsoulteFolderPath());
      else
        Log.Info("Папка не найдена.");
    }

    public override bool CanExecute(IManga manga)
    {
      return manga != null;
    }

    public OpenFolderCommand()
    {
      this.Name = "Открыть папку";
    }
  }
}