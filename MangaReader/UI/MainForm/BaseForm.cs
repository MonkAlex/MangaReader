using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Shell;
using Hardcodet.Wpf.TaskbarNotification;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.Services;

namespace MangaReader.UI.MainForm
{
  /// <summary>
  /// Interaction logic for BaseForm.xaml
  /// </summary>
  public partial class BaseForm : Window
  {
    public TaskbarIcon NotifyIcon = new TaskbarIcon{IsEnabled = false};

    public ListCollectionView View { get; set; }

    public BaseForm()
    {
      this.DataContext = this;
      this.TaskbarItemInfo = new TaskbarItemInfo();
      Command.AddMainMenuCommands(this);
      Command.AddMangaCommands(this);
      this.Initialized += (sender, args) => Library.Initialize(this);

      this.Loaded += (sender, args) =>
      {
        NotifyIconInitialize();
      };

      this.Closing += (sender, args) =>
      {
        this.NotifyIcon.IsEnabled = false;
        this.NotifyIcon.Dispose();
        Application.Current.Shutdown(0);
      };

      View = new ListCollectionView(Library.LibraryMangas)
      {
        Filter = Filter,
        CustomSort = new MangasComparer()
      };
    }

    internal virtual bool Filter(object o)
    {
      var manga = o as Mangas;
      if (manga == null)
        return false;

      if (LibraryFilter.Uncompleted && manga.IsCompleted)
        return false;

      if (LibraryFilter.OnlyUpdate && !manga.NeedUpdate)
        return false;

      return LibraryFilter.AllowedTypes.Any(t => (t.Value as MangaSetting).Manga == manga.GetType().MangaType()) &&
        manga.Name.ToLowerInvariant().Contains(LibraryFilter.Name.ToLowerInvariant());
    }

    private void NotifyIconInitialize()
    {
      Command.AddMainMenuCommands(this.NotifyIcon);
      Command.AddMangaCommands(this.NotifyIcon);
      this.NotifyIcon.ToolTipText = Strings.Title;
      this.NotifyIcon.Icon = Properties.Resources.main;
      this.NotifyIcon.TrayMouseDoubleClick += NotifyIcon_OnTrayMouseDoubleClick;
      this.NotifyIcon.TrayBalloonTipClicked += NotifyIcon_OnTrayBalloonTipClicked;

      var add = new MenuItem() { Header = Strings.Library_Action_Add, Command = ApplicationCommands.New };
      var settings = new MenuItem() { Command = Command.ShowSettings };
      var selfUpdate = new MenuItem() { Command = Command.CheckUpdates };
      var exit = new MenuItem() { Command = ApplicationCommands.Close };

      var menu = new ContextMenu();
      menu.CommandBindings.Clear();
      menu.CommandBindings.AddRange(this.NotifyIcon.CommandBindings);
      menu.Items.Add(add);
      menu.Items.Add(settings);
      menu.Items.Add(selfUpdate);
      menu.Items.Add(exit);
      this.NotifyIcon.ContextMenu = menu;

      this.NotifyIcon.IsEnabled = true;
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
        Command.OpenFolder.Execute(downloadable, element);
    }

  }
}
