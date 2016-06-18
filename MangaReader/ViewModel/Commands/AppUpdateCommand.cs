using MangaReader.Core.Update;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands
{
  public class AppUpdateCommand : LibraryBaseCommand
  {
    public override void Execute(object parameter)
    {
      var title = "Обновление";

      if (Updater.CheckUpdate())
      {
        var note = string.Format("Доступно обновление с версии {0} на {1}", Updater.ClientVersion, Updater.ServerVersion);
        if (Dialogs.ShowYesNoDialog(title, "Запустить процесс обновления?", note))
          new DownloadUpdate().Show();
      }
      else
      {
        Dialogs.ShowInfo(title, "Обновлений не найдено.");
      }
    }

    public AppUpdateCommand()
    {
      this.Name = Strings.Library_CheckUpdate;
      this.Icon = "pack://application:,,,/Icons/Main/update_app.png";
    }
  }
}