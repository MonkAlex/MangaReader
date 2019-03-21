using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Manga
{
  public class OpenFolderCommandBase : BaseCommand
  {

    public override Task Execute(object parameter)
    {
      this.Execute(parameter as IDownloadable);
      return Task.CompletedTask;
    }

    public void Execute(IDownloadable parameter)
    {
      if (parameter != null && Directory.Exists(parameter.GetAbsoluteFolderPath()))
        Helper.StartUseShell(parameter.GetAbsoluteFolderPath());
      else
        Log.Info(Strings.Library_Status_FolderNotFound);
    }

    public override bool CanExecute(object parameter)
    {
      return true;
    }

    public OpenFolderCommandBase()
    {
      this.Name = Strings.Manga_Action_OpenFolder;
      this.Icon = "pack://application:,,,/Icons/Manga/open_folder.png";
    }
  }
}