using System;
using System.ComponentModel;
using System.Windows;
using MangaReader.Services.Config;
using MangaReader.ViewModel.Commands;

namespace MangaReader.UI
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void Table_OnStateChanged(object sender, EventArgs e)
    {
      if (ConfigStorage.Instance.AppConfig.MinimizeToTray && this.WindowState == System.Windows.WindowState.Minimized)
        this.Hide();
    }

    private void Window_OnClosing(object sender, CancelEventArgs e)
    {
      ConfigStorage.Instance.ViewConfig.SaveWindowState(this);
      new ExitCommand().Execute(this);
    }
  }
}
