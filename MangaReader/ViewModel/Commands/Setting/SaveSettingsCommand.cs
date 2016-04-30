using MangaReader.Core.Services.Config;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel.Commands.Setting
{
  public class SaveSettingsCommand : BaseCommand
  {
    private readonly SettingModel settingModel;

    public override void Execute(object parameter)
    {
      base.Execute(parameter);

      ConfigStorage.Instance.AppConfig.Language = settingModel.Language;
      ConfigStorage.Instance.AppConfig.UpdateReader = settingModel.UpdateReader;
      ConfigStorage.Instance.AppConfig.MinimizeToTray = settingModel.MinimizeToTray;

      int hour;
      if (int.TryParse(settingModel.AutoUpdateHours, out hour))
        ConfigStorage.Instance.AppConfig.AutoUpdateInHours = hour;

      ConfigStorage.Instance.Save();

      var window = ViewService.Instance.TryGet(settingModel);
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