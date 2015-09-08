﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shell;
using Hardcodet.Wpf.TaskbarNotification;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.Services;
using NHibernate.Linq;

namespace MangaReader.UI.MainForm
{
  /// <summary>
  /// Interaction logic for BaseForm.xaml
  /// </summary>
  public partial class BaseForm : Window
  {
    public TaskbarIcon NotifyIcon = new TaskbarIcon{IsEnabled = false};

    public BaseForm()
    {
      this.TaskbarItemInfo = new TaskbarItemInfo();
      Command.AddMainMenuCommands(this);
      Command.AddMangaCommands(this);
      this.Initialized += (sender, args) => Library.Initialize(this);

      this.Loaded += (sender, args) =>
      {
        Command.AddMainMenuCommands(this.NotifyIcon);
        this.NotifyIcon.ToolTipText = Strings.Title;
        this.NotifyIcon.Icon = Properties.Resources.main;
        this.NotifyIcon.TrayMouseDoubleClick += NotifyIcon_OnTrayMouseDoubleClick;
        this.NotifyIcon.TrayBalloonTipClicked += NotifyIcon_OnTrayBalloonTipClicked;
        this.NotifyIcon.TrayRightMouseUp += NotifyIcon_OnTrayRightMouseUp;
        this.NotifyIcon.IsEnabled = true;
      };

      this.Closing += (sender, args) =>
      {
        this.NotifyIcon.IsEnabled = false;
        this.NotifyIcon.Dispose();
        Application.Current.Shutdown(0);
      };
    }


    private void NotifyIcon_OnTrayMouseDoubleClick(object sender, RoutedEventArgs e)
    {
      if (Settings.MinimizeToTray)
      {
        this.Show();
        this.WindowState = WindowState.Normal;
      }
    }

    private void NotifyIcon_OnTrayBalloonTipClicked(object sender, RoutedEventArgs e)
    {
      var element = sender as FrameworkElement;
      if (element == null)
        return;

      var downloadable = element.DataContext as IDownloadable;
      if (downloadable != null)
        Command.OpenFolder.Execute(downloadable, null);
    }

    private void NotifyIcon_OnTrayRightMouseUp(object sender, RoutedEventArgs e)
    {
      var item = sender as TaskbarIcon;

      var add = new MenuItem() { Header = Strings.Library_Action_Add, Command = ApplicationCommands.New };
      var settings = new MenuItem() { Header = Strings.Library_Action_Settings, Command = Command.ShowSettings };
      var selfUpdate = new MenuItem() { Header = Strings.Library_CheckUpdate, IsEnabled = Library.IsAvaible };
      selfUpdate.Click += (o, agrs) => Update.StartUpdate();
      var exit = new MenuItem() { Header = Strings.Library_Exit, Command = ApplicationCommands.Close };

      var menu = new ContextMenu();
      menu.CommandBindings.Clear();
      menu.CommandBindings.AddRange(item.CommandBindings);
      menu.Items.Add(add);
      menu.Items.Add(settings);
      menu.Items.Add(selfUpdate);
      menu.Items.Add(exit);
      item.ContextMenu = menu;
    }
  }
}