using MangaReader.Core.Update;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.ViewModel.Commands.Primitives;
using Ookii.Dialogs.Wpf;

namespace MangaReader.ViewModel.Commands
{
  public class AppUpdateCommand : LibraryBaseCommand
  {
    public override string Name { get { return Strings.Library_CheckUpdate; } }

    public override void Execute(object parameter)
    {
      var dialog = new TaskDialog();
      dialog.WindowTitle = "Обновление";
      var owner = WindowHelper.Owner;
      if (Updater.CheckUpdate())
      {
        dialog.MainInstruction = "Запустить процесс обновления?";
        dialog.Content = string.Format("Доступно обновление с версии {0} на {1}", Updater.ClientVersion.ToString(3), Updater.ServerVersion.ToString(3));
        dialog.Buttons.Add(new TaskDialogButton(ButtonType.Yes));
        dialog.Buttons.Add(new TaskDialogButton(ButtonType.No));
        if (dialog.ShowDialog(owner).ButtonType == ButtonType.Yes)
        {
          var updateModel = new DownloadUpdate(new Converting(owner));
          updateModel.Show();
        }
      }
      else
      {
        dialog.MainInstruction = "Обновлений не найдено.";
        dialog.Buttons.Add(new TaskDialogButton(ButtonType.Ok));
        dialog.ShowDialog(owner);
      }
    }
  }
}