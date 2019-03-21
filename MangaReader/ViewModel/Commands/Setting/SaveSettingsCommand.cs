using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.Services;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Commands.Primitives;
using MangaReader.ViewModel.Setting;

namespace MangaReader.ViewModel.Commands.Setting
{
  public class SaveSettingsCommand : BaseCommand
  {
    private readonly SettingModel settingModel;

    public override async Task Execute(object parameter)
    {
      var skinGuid = ConfigStorage.Instance.ViewConfig.SkinGuid;

      try
      {
        await Task.WhenAll(settingModel.Views.Select(v => v.Save()).ToArray()).ConfigureAwait(true);

        ConfigStorage.Instance.Save();
      }
      catch (Exception e)
      {
        Log.Exception(e);
        var message = e.InnerException?.Message ?? e.Message;
        Dialogs.ShowInfo("Не удалось сохранить настройки", message);
      }

      if (skinGuid != ConfigStorage.Instance.ViewConfig.SkinGuid)
      {
        var restart = Dialogs.ShowYesNoDialog("Перезапуск", "Перезапустить программу?", "Необходим перезапуск для применения новых настроек.");
        if (restart)
        {
          // TODO: потенциальная гонка условий, может не запуститься.
          await new ExitCommand().Execute(parameter).ConfigureAwait(true);
          Helper.StartUseShell(Application.ResourceAssembly.Location);
        }
      }

      var window = ViewService.Instance.TryGet<Window>(settingModel);
      if (window != null)
        window.Close();

      ValidateMangaPaths();
    }

    public static void ValidateMangaPaths()
    {
      var paths = Core.NHibernate.Repository.GetStateless<MangaSetting>().ToList()
        .Where(p =>
        {
          var absolute = DirectoryHelpers.GetAbsoluteFolderPath(p.Folder);
          return !DirectoryHelpers.Equals(AppConfig.DownloadFolder, absolute) && !Directory.Exists(absolute);
        })
        .ToList();
      if (paths.Any())
        Dialogs.OpenSettingsOnPathNotExists(paths);
    }

    public SaveSettingsCommand(SettingModel model)
    {
      this.settingModel = model;
      this.Name = "Сохранить";
    }
  }
}