using System.Diagnostics;
using System.Windows;
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

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      var skinChanged = ConfigStorage.Instance.ViewConfig.SkinGuid != settingModel.AppSetting.Skin.Guid;

      foreach (var mangaSetting in settingModel.Views)
        mangaSetting.Save();

      ConfigStorage.Instance.Save();

      if (skinChanged)
      {
        var restart = Dialogs.ShowYesNoDialog("Перезапуск", "Перезапустить программу?", "Необходим перезапуск для применения новых настроек.");
        if (restart)
        {
          // TODO: потенциальная гонка условий, может не запуститься.
          new ExitCommand().Execute(parameter);
          Process.Start(Application.ResourceAssembly.Location);
        }
      }

      var window = ViewService.Instance.TryGet<System.Windows.Window>(settingModel);
      if (window != null)
        window.Close();
    }

    public SaveSettingsCommand(SettingModel model)
    {
      this.settingModel = model;
      this.Name = "Сохранить";
    }
  }
}