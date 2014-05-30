using System;
using System.Windows;
using Ookii.Dialogs.Wpf;

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
            UpdateBox.IsChecked = Settings.Update;
            UpdateReaderBox.IsChecked = Settings.UpdateReader;
            this.CompressBox.IsChecked = Settings.CompressManga;
            this.DownloadPath.Text = Settings.DownloadFolder;
            this.Language.SelectedItem = Settings.Language;
        }

        private void ChangeFolder(object sender, RoutedEventArgs e)
        {
            var dialog = new VistaFolderBrowserDialog();
            dialog.ShowDialog();
            Settings.DownloadFolder = dialog.SelectedPath + "\\";
            this.DownloadPath.Text = Settings.DownloadFolder;
        }

        private void SettingsForm_OnClosed(object sender, EventArgs e)
        {
            Settings.Language = (Settings.Languages)this.Language.SelectedItem;
            Settings.Update = UpdateBox.IsChecked.Value;
            Settings.UpdateReader = UpdateReaderBox.IsChecked.Value;
            Settings.CompressManga = CompressBox.IsChecked.Value;

            Settings.Save();
        }
    }
}
