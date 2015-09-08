using System;
using System.Windows;
using MangaReader.Services;

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
      this.UpdateReaderBox.IsChecked = Settings.UpdateReader;
      this.MinimizeToTray.IsChecked = Settings.MinimizeToTray;
      this.MangaLanguage.SelectedItem = Settings.Language;
      this.AutoUpdate.Text = Settings.AutoUpdateInHours.ToString();
      foreach (var setting in Settings.MangaSettings)
      {
        this.Tabs.Items.Add(new MangaSettings() { DataContext = setting });
      }
    }

    private void SettingsForm_OnClosed(object sender, EventArgs e)
    {
      Settings.Language = (Settings.Languages)this.MangaLanguage.SelectedItem;
      Settings.UpdateReader = UpdateReaderBox.IsChecked.Value;
      Settings.MinimizeToTray = MinimizeToTray.IsChecked.Value;

      int hour;
      var hoursInt = int.TryParse(this.AutoUpdate.Text, out hour);
      Settings.AutoUpdateInHours = hoursInt ? hour : Settings.AutoUpdateInHours;

      Settings.Save();
    }
  }
}
