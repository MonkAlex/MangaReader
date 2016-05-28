using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MangaReader.Core.Services.Config;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Commands.Setting;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel.Setting
{
  public class SettingModel : BaseViewModel
  {
    private SettingViewModel selectedModel;
    public AppSettingModel AppSetting { get; private set; }

    public ObservableCollection<SettingViewModel> Views { get; private set; }

    public SettingViewModel SelectedModel
    {
      get { return selectedModel; }
      set
      {
        SyncLoginOnModelChange(selectedModel);
        selectedModel = value;
        OnPropertyChanged();
      }
    }

    public ICommand Save { get; private set; }

    public SettingModel()
    {
      this.AppSetting = new AppSettingModel();
      this.Views = new ObservableCollection<SettingViewModel>();
      this.Save = new SaveSettingsCommand(this);

      this.Views.Add(this.AppSetting);
      foreach (var setting in ConfigStorage.Instance.DatabaseConfig.MangaSettings)
        this.Views.Add(new MangaSettingModel(setting));
      this.SelectedModel = this.AppSetting;
    }

    private void SyncLoginOnModelChange(SettingViewModel model)
    {
      var mangaSettings = model as MangaSettingModel;
      if (mangaSettings != null)
      {
        var someLoginView = this.Views
          .OfType<MangaSettingModel>()
          .Where(m => !Equals(m, mangaSettings) && Equals(m.LoginId, mangaSettings.LoginId))
          .ToList();
        foreach (var settingModel in someLoginView)
        {
          settingModel.Login = mangaSettings.Login;
          settingModel.Password = mangaSettings.Password;
        }
      }
    }

    public override void Show()
    {
      base.Show();

      var window = ViewService.Instance.TryGet<System.Windows.Window>(this);
      if (window != null)
      {
        window.ShowDialog();
      }
    }
  }
}