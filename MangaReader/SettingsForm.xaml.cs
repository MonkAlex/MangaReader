using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using MangaReader.Core.Services.Config;
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
      Owner = WindowHelper.Owner;
      InitializeComponent();
      ViewModel.Primitive.BaseViewModel.SubToViewModel(this);
      /*
      this.UpdateReaderBox.IsChecked = ConfigStorage.Instance.AppConfig.UpdateReader;
      this.MinimizeToTray.IsChecked = ConfigStorage.Instance.AppConfig.MinimizeToTray;
      this.MangaLanguage.SelectedItem = ConfigStorage.Instance.AppConfig.Language;
      this.AutoUpdate.Text = ConfigStorage.Instance.AppConfig.AutoUpdateInHours.ToString();
      foreach (var setting in ConfigStorage.Instance.DatabaseConfig.MangaSettings)
      {
        this.Tabs.Items.Add(new TabItem() {Content = new MangaSettings() { DataContext = setting }, Header = setting.MangaName });
      }
      */
    }
    /*
    private void OkButton_OnClick(object sender, RoutedEventArgs e)
    {
      try
      {
        ConfigStorage.Instance.AppConfig.Language = (Languages)this.MangaLanguage.SelectedItem;
        ConfigStorage.Instance.AppConfig.UpdateReader = UpdateReaderBox.IsChecked.Value;
        ConfigStorage.Instance.AppConfig.MinimizeToTray = MinimizeToTray.IsChecked.Value;

        int hour;
        var hoursInt = int.TryParse(this.AutoUpdate.Text, out hour);
        ConfigStorage.Instance.AppConfig.AutoUpdateInHours = hoursInt ? hour : ConfigStorage.Instance.AppConfig.AutoUpdateInHours;

        ConfigStorage.Instance.Save();

        this.DialogResult = true;
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
    }

    private void SettingsForm_OnClosing(object sender, CancelEventArgs e)
    {
      if (this.DialogResult != true)
        ConfigStorage.Instance.DatabaseConfig.MangaSettings.Update();
    }
    */
  }
}
