using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using MangaReader.Account;
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
      Convert();
      Settings.Load();
      Update.Initialize();
      InitializeComponent();
      Settings.UpdateWindowsState(this);
      Library.Initialize(this);
      Initialize();
    }

    /// <summary>
    /// Инициализация программмы.
    /// </summary>
    public void Initialize()
    {
      ServicePointManager.DefaultConnectionLimit = 100;
      Grouple.LoginWhile();
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
    /// Сконвертировать старый формат в новый.
    /// </summary>
    private static void Convert()
    {
      Settings.Convert();
      Cache.Convert();
      History.Convert();
      Library.Convert();
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

      var manga = ((ListViewItem)sender).DataContext as Mangas;
      if (manga == null)
        return;

      if (Directory.Exists(manga.Folder))
        Process.Start(manga.Folder);
      else
        Library.Status = Strings.Library_Status_FolderNotFound;
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
        Library.Add(manga.Url);
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
      view.Click += (o, agrs) => Process.Start(manga.Url);
      var needUpdate = new MenuItem() { Header = manga.NeedUpdate ? Strings.Manga_NotUpdate : Strings.Manga_Update, IsEnabled = this.IsAvaible };
      needUpdate.Click += (o, args) => manga.NeedUpdate = !manga.NeedUpdate;

      var menu = new ContextMenu();
      menu.Items.Add(openFolder);
      menu.Items.Add(needUpdate);
      menu.Items.Add(update);
      menu.Items.Add(view);
      menu.Items.Add(remove);
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