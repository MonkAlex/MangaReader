using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using MangaReader.Core.Manga;
using MangaReader.Core.Services;
using MangaReader.Properties;
using MangaReader.ViewModel.Commands;
using MangaReader.ViewModel.Commands.Manga;
using MangaReader.ViewModel.Commands.Primitives;

namespace MangaReader.ViewModel
{
  public class TaskbarIconModel : Primitive.NotifyPropertyChanged, IDisposable
  {
    private TaskbarIcon icon;
    private string toolTipText;
    private ICommand doubleClickCommand;
    private ObservableCollection<ContentMenuItem> contextMenu;

    public string ToolTipText
    {
      get { return toolTipText; }
      set
      {
        toolTipText = value;
        OnPropertyChanged();
      }
    }

    public ICommand DoubleClickCommand
    {
      get { return doubleClickCommand; }
      set
      {
        doubleClickCommand = value;
        OnPropertyChanged();
      }
    }

    public ObservableCollection<ContentMenuItem> ContextMenu
    {
      get { return contextMenu; }
      set
      {
        contextMenu = value;
        OnPropertyChanged();
      }
    }

    public IDownloadable LastShowed { get; private set; }

    private void NotifyIconInitialize(LibraryViewModel library)
    {
      ToolTipText = Strings.Title;
      ContextMenu.Add(new ContentMenuItem(new AddNewMangaCommand(library)));
      ContextMenu.Add(new ContentMenuItem(new ShowSettingCommand(library)));
      ContextMenu.Add(new ContentMenuItem(new AppUpdateCommand(library)));
      ContextMenu.Add(new ContentMenuItem(new ExitCommand()));
      this.icon.TrayBalloonTipClicked += (sender, args) =>
      {
        if (LastShowed != null)
          new OpenFolderCommand(library).Execute(LastShowed);
      };
    }

    public void ShowInTray(string message)
    {
      ShowInTray(message, null);
    }

    public void ShowInTray(string message, object context)
    {
      icon.ShowBalloonTip(Strings.Title, message, BalloonIcon.Info);
      LastShowed = context as IDownloadable;
    }

    public TaskbarIconModel(object icon, LibraryViewModel library)
    {
      this.icon = icon as TaskbarIcon;
      this.ContextMenu = new ObservableCollection<ContentMenuItem>();
      this.DoubleClickCommand = new ShowMainWindowCommand();
      NotifyIconInitialize(library);
    }

    public void Dispose()
    {
      if (icon != null)
        icon.Dispose();
    }
  }
}