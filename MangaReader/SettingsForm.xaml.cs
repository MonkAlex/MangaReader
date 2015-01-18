using System;
using System.Linq;
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
      this.UpdateBox.IsChecked = Settings.Update;
      this.UpdateReaderBox.IsChecked = Settings.UpdateReader;
      this.MinimizeToTray.IsChecked = Settings.MinimizeToTray;
      this.CompressBox.IsChecked = Settings.CompressManga;
      this.MangaLanguage.SelectedItem = Settings.Language;
      this.AutoUpdate.Text = Settings.AutoUpdateInHours.ToString();
      this.ReadManga.DataContext = Settings.DownloadFolders.First(x => x.Manga == Manga.Grouple.Readmanga.Type);
      this.Acomics.DataContext = Settings.DownloadFolders.First(x => x.Manga == Manga.Acomic.Acomics.Type);
    }

    private void SettingsForm_OnClosed(object sender, EventArgs e)
    {
      Settings.Language = (Settings.Languages)this.MangaLanguage.SelectedItem;
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
