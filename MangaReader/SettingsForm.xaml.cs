using System;
using System.Windows;
using MangaReader.Manga;

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
      this.UpdateBox.IsChecked = Settings.Update;
      this.UpdateReaderBox.IsChecked = Settings.UpdateReader;
      this.MinimizeToTray.IsChecked = Settings.MinimizeToTray;
      this.CompressBox.IsChecked = Settings.CompressManga;
      this.Language.SelectedItem = Settings.Language;
      this.AutoUpdate.Text = Settings.AutoUpdateInHours.ToString();
      this.ReadManga.DataContext = typeof(Readmanga);
      this.Acomics.DataContext = typeof(Acomics);
    }

    private void SettingsForm_OnClosed(object sender, EventArgs e)
    {
      Settings.Language = (Settings.Languages)this.Language.SelectedItem;
      Settings.Update = UpdateBox.IsChecked.Value;
      Settings.UpdateReader = UpdateReaderBox.IsChecked.Value;
      Settings.MinimizeToTray = MinimizeToTray.IsChecked.Value;
      Settings.CompressManga = CompressBox.IsChecked.Value;

      int hour;
      var hoursInt = int.TryParse(this.AutoUpdate.Text, out hour);
      Settings.AutoUpdateInHours = hoursInt ? hour : Settings.AutoUpdateInHours;

      Settings.Save();
    }
  }
}
