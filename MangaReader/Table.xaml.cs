using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.Services;
using ThreadState = System.Threading.ThreadState;

namespace MangaReader
{
  /// <summary>
  /// Логика взаимодействия для Table.xaml
  /// </summary>
  public partial class Table : Window
  {
    /// <summary>
    /// Таймер на обновление формы.
    /// </summary>
    // ReSharper disable once NotAccessedField.Local
    private static DispatcherTimer _timer;

    /// <summary>
    /// Поток загрузки манги.
    /// </summary>
    private static Thread _loadThread;

    /// <summary>
    /// Библиотека доступна, т.е. не в процессе обновления.
    /// </summary>
    internal bool IsAvaible = true;

    public Table()
    {
      InitializeComponent();
      Settings.UpdateWindowsState(this);
      Library.Initialize(this);
      _timer = new DispatcherTimer(new TimeSpan(0, 0, 1),
          DispatcherPriority.Background,
          TimerTick,
          Dispatcher.CurrentDispatcher);
    }

    void _PreviewMouseMoveEvent(object sender, MouseEventArgs e)
    {
      if (!this.IsAvaible)
        return;

      if (!(sender is ListViewItem) || e.LeftButton != MouseButtonState.Pressed)
        return;

      var draggedItem = sender as ListViewItem;
      DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
      draggedItem.IsSelected = true;
    }

    void Library_Drop(object sender, DragEventArgs e)
    {
      if (!this.IsAvaible)
        return;

      var droppedData = e.Data.GetData(e.Data.GetFormats().FirstOrDefault()) as Mangas;
      var target = ((ListViewItem)(sender)).DataContext as Mangas;

      var removedIdx = this.FormLibrary.Items.IndexOf(droppedData);
      var targetIdx = this.FormLibrary.Items.IndexOf(target);

      if (removedIdx < targetIdx)
      {
        Library.DatabaseMangas.Insert(targetIdx + 1, droppedData);
        Library.DatabaseMangas.RemoveAt(removedIdx);
      }
      else
      {
        var remIdx = removedIdx + 1;
        if (Library.DatabaseMangas.Count + 1 <= remIdx)
          return;
        Library.DatabaseMangas.Insert(targetIdx, droppedData);
        Library.DatabaseMangas.RemoveAt(remIdx);
      }
    }

    /// <summary>
    /// Обновление библиотеки.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Update_click(object sender, RoutedEventArgs e)
    {
      if (_loadThread == null || _loadThread.ThreadState == ThreadState.Stopped)
        _loadThread = new Thread(() => Library.Update());
      if (_loadThread.ThreadState == ThreadState.Unstarted)
        _loadThread.Start();
    }

    /// <summary>
    /// Обработчик двойного клика по манге.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Mangas_clicked(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount < 2 || !(sender is ListViewItem))
        return;

      var downloadable = ((ListViewItem)sender).DataContext as IDownloadable;
      if (downloadable == null)
        return;

      MenuOpenFolder(downloadable);
    }

    /// <summary>
    /// Добавление манги. Используем кастомный диалог.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Add_click(object sender, RoutedEventArgs e)
    {
      var db = new Input { Owner = this };
      if (db.ShowDialog() != true)
        return;

      if (!string.IsNullOrWhiteSpace(db.Result.Text))
        Library.Add(db.Result.Text);
      foreach (var manga in db.Bookmarks.SelectedItems.OfType<Mangas>())
      {
        Library.Add(manga.Uri);
      }
    }

    /// <summary>
    /// Настройки.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Settings_click(object sender, RoutedEventArgs e)
    {
      new SettingsForm { Owner = this }.ShowDialog();
    }

    /// <summary>
    /// Обработчик таймера, вешаем всякие обработки формы.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TimerTick(object sender, EventArgs e)
    {
      this.IsAvaible = _loadThread == null || _loadThread.ThreadState == ThreadState.Stopped;
      this.TextBlock.Text = Library.Status;
      UpdateButton.IsEnabled = this.IsAvaible;
      AddButton.IsEnabled = this.IsAvaible;
      SettingsButton.IsEnabled = this.IsAvaible;

      if (this.IsAvaible && Settings.AutoUpdateInHours > 0 &&
        DateTime.Now > Settings.LastUpdate.AddHours(Settings.AutoUpdateInHours))
      {
        Log.Add(Strings.AutoUpdate);
        Update_click(this, new RoutedEventArgs());
      }
    }

    /// <summary>
    /// Клик правой кнопкой.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ListViewItem_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
    {
      var item = sender as ListViewItem;

      var manga = item.DataContext as Mangas;
      if (manga == null)
        return;

      var openFolder = new MenuItem() { Header = Strings.Manga_Action_OpenFolder, FontWeight = FontWeights.Bold };
      openFolder.Click += (o, args) => MenuOpenFolder(manga);
      var update = new MenuItem() { Header = Strings.Manga_Action_Update, IsEnabled = this.IsAvaible };
      update.Click += (o, agrs) => UpdateManga(manga);
      var remove = new MenuItem() { Header = Strings.Manga_Action_Remove, IsEnabled = this.IsAvaible };
      remove.Click += (o, agrs) => Library.Remove(manga);
      var view = new MenuItem() { Header = Strings.Manga_Action_View };
      view.Click += (o, agrs) => Process.Start(manga.Uri.OriginalString);
      var needUpdate = new MenuItem() { Header = manga.NeedUpdate ? Strings.Manga_NotUpdate : Strings.Manga_Update, IsEnabled = this.IsAvaible };
      needUpdate.Click += (o, args) => { manga.NeedUpdate = !manga.NeedUpdate; manga.Save(); };
      var settings = new MenuItem() { Header = Strings.Manga_Settings, IsEnabled = this.IsAvaible };
      settings.Click += (o, args) => new MangaForm { DataContext = manga, Owner = this }.ShowDialog();

      var menu = new ContextMenu();
      menu.Items.Add(openFolder);
      menu.Items.Add(needUpdate);
      menu.Items.Add(update);
      menu.Items.Add(view);
      menu.Items.Add(remove);
      menu.Items.Add(settings);
      item.ContextMenu = menu;
    }

    private static void UpdateManga(Mangas manga)
    {
      if (_loadThread == null || _loadThread.ThreadState == ThreadState.Stopped)
        _loadThread = new Thread(() => Library.Update(manga));
      if (_loadThread.ThreadState == ThreadState.Unstarted)
        _loadThread.Start();
    }

    private static void MenuOpenFolder(IDownloadable manga)
    {
      if (Directory.Exists(manga.Folder))
        Process.Start(manga.Folder);
      else
        Library.Status = Strings.Library_Status_FolderNotFound;
    }

    private void Window_OnClosing(object sender, CancelEventArgs e)
    {
      Settings.WindowsState = new object[] { this.Top, this.Left, this.Width, this.Height, this.WindowState };
      this.NotifyIcon.Dispose();
      Application.Current.Shutdown(0);
    }

    private void ListView_MouseDown(object sender, MouseButtonEventArgs e)
    {
      var listView = sender as ListView;
      if (listView != null)
      {
        listView.SelectedIndex = -1;
      }
    }

    private void Table_OnStateChanged(object sender, EventArgs e)
    {
      if (Settings.MinimizeToTray && this.WindowState == WindowState.Minimized)
        this.Hide();
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
        MenuOpenFolder(downloadable);
    }

    private void NotifyIcon_OnTrayRightMouseUp(object sender, RoutedEventArgs e)
    {
      var item = sender as TaskbarIcon;

      var update = new MenuItem() { Header = Strings.Manga_Action_Update, IsEnabled = this.IsAvaible };
      update.Click += (o, agrs) => this.Update_click(sender, e);
      var add = new MenuItem() { Header = Strings.Library_Action_Add, IsEnabled = this.IsAvaible };
      add.Click += (o, agrs) => this.Add_click(sender, e);
      var settings = new MenuItem() { Header = Strings.Library_Action_Settings, IsEnabled = this.IsAvaible };
      settings.Click += (o, agrs) => this.Settings_click(sender, e);
      var selfUpdate = new MenuItem() { Header = Strings.Library_CheckUpdate, IsEnabled = this.IsAvaible };
      selfUpdate.Click += (o, agrs) => Update.StartUpdate();
      var exit = new MenuItem() { Header = Strings.Library_Exit };
      exit.Click += (o, agrs) => Application.Current.Shutdown(0);

      var menu = new ContextMenu();
      menu.Items.Add(update);
      menu.Items.Add(add);
      menu.Items.Add(settings);
      menu.Items.Add(selfUpdate);
      menu.Items.Add(exit);
      item.ContextMenu = menu;
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class UrlTypeConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      var result = value == null ? string.Empty : value.ToString();
      if (result.Contains("readmanga"))
        return "RM";
      if (result.Contains("adultmanga"))
        return "AM";
      if (result.Contains("acomics"))
        return "AC";
      return "NA";
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class StringImageConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      var result = "Icons/play.png";
      switch (value != null ? value.ToString() : string.Empty)
      {
        case "завершен":
          result = "Icons/stop.png";
          break;

        case "True":
          result = "Icons/yes.png";
          break;

        case "False":
          result = "Icons/no.png";
          break;
      }
      return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion
  }
}