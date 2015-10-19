using System;
using System.Windows;
using MangaReader.Services;
using MangaReader.Services.Config;

namespace MangaReader
{
  /// <summary>
  /// Логика взаимодействия для SettingsForm.xaml
  /// </summary>
  public partial class SettingsForm : Window
  {
    public SettingsForm()
    {
      InitializeComponent();
      this.UpdateReaderBox.IsChecked = ConfigStorage.Instance.AppConfig.UpdateReader;
      this.MinimizeToTray.IsChecked = ConfigStorage.Instance.AppConfig.MinimizeToTray;
      this.MangaLanguage.SelectedItem = ConfigStorage.Instance.AppConfig.Language;
      this.AutoUpdate.Text = ConfigStorage.Instance.AppConfig.AutoUpdateInHours.ToString();
      foreach (var setting in ConfigStorage.Instance.DatabaseConfig.MangaSettings)
      {
        this.Tabs.Items.Add(new MangaSettings() { DataContext = setting });
      }
    }

    private void SettingsForm_OnClosed(object sender, EventArgs e)
    {
      ConfigStorage.Instance.AppConfig.Language = (Languages)this.MangaLanguage.SelectedItem;
      ConfigStorage.Instance.AppConfig.UpdateReader = UpdateReaderBox.IsChecked.Value;
      ConfigStorage.Instance.AppConfig.MinimizeToTray = MinimizeToTray.IsChecked.Value;

      int hour;
      var hoursInt = int.TryParse(this.AutoUpdate.Text, out hour);
      ConfigStorage.Instance.AppConfig.AutoUpdateInHours = hoursInt ? hour : ConfigStorage.Instance.AppConfig.AutoUpdateInHours;

      ConfigStorage.Instance.Save();
    }
  }
}
