using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MangaReader.Core.Services.Config;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Commands.Setting;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel
{
  public class SettingModel : BaseViewModel
  {
    private bool updateReader;
    private bool minimizeToTray;
    private Languages language;
    private string autoUpdateHours;

    public bool UpdateReader
    {
      get { return updateReader; }
      set
      {
        updateReader = value;
        OnPropertyChanged();
      }
    }

    public bool MinimizeToTray
    {
      get { return minimizeToTray; }
      set
      {
        minimizeToTray = value;
        OnPropertyChanged();
      }
    }

    public Languages Language
    {
      get { return language; }
      set
      {
        language = value;
        OnPropertyChanged();
      }
    }

    public List<Languages> Languages { get; private set; }

    public string AutoUpdateHours
    {
      get { return autoUpdateHours; }
      set
      {
        autoUpdateHours = value;
        OnPropertyChanged();
      }
    }

    public ObservableCollection<MangaSettingModel> MangaSetting { get; private set; }

    public ICommand Save { get; private set; }

    public SettingModel()
    {
      this.Languages = new List<Languages>(System.Enum.GetValues(typeof(Languages)).OfType<Languages>());
      this.MangaSetting = new ObservableCollection<MangaSettingModel>();
      this.Save = new SaveSettingsCommand(this);
    }

    public override void Load()
    {
      base.Load();

      this.UpdateReader = ConfigStorage.Instance.AppConfig.UpdateReader;
      this.MinimizeToTray = ConfigStorage.Instance.AppConfig.MinimizeToTray;
      this.Language = ConfigStorage.Instance.AppConfig.Language;
      this.AutoUpdateHours = ConfigStorage.Instance.AppConfig.AutoUpdateInHours.ToString();
    }

    public override void Show()
    {
      base.Show();

      var window = ViewService.Instance.TryGet(this);
      if (window != null)
      {
        window.ShowDialog();
      }
    }
  }
}