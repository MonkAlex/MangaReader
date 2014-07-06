using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MangaReader.Logins;
using MangaReader.Properties;
using MangaReader.Services;
using ThreadState = System.Threading.ThreadState;

namespace MangaReader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
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

        public MainWindow()
        {
            Settings.Load();
            Update.Initialize();
            InitializeComponent();
            Settings.UpdateWindowsState(this);
            Initialize();
        }

        /// <summary>
        /// Инициализация программмы.
        /// </summary>
        public void Initialize()
        {
            _timer = new DispatcherTimer(new TimeSpan(0, 0, 1),
                DispatcherPriority.Background,
                TimerTick,
                Dispatcher.CurrentDispatcher);
            this.FormLibrary.ItemsSource = Library.Initialize(this.TaskBar);
            Convert();
            Grouple.LoginWhile();
        }

        void _PreviewMouseMoveEvent(object sender, MouseEventArgs e)
        {
            if (!(sender is ListBoxItem) || e.LeftButton != MouseButtonState.Pressed)
                return;

            var draggedItem = sender as ListBoxItem;
            DragDrop.DoDragDrop(draggedItem, draggedItem.DataContext, DragDropEffects.Move);
            draggedItem.IsSelected = true;
        }

        void Library_Drop(object sender, DragEventArgs e)
        {
            var droppedData = e.Data.GetData(typeof(Manga)) as Manga;
            var target = ((ListBoxItem)(sender)).DataContext as Manga;

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
            var listBox = sender as ListBox;
            if (listBox == null || listBox.SelectedItems.Count == 0 || !(e.MouseDevice.DirectlyOver is Image))
                return;

            var manga = listBox.SelectedItem as Manga;
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
            foreach (var manga in db.Bookmarks.SelectedItems.OfType<Manga>())
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
            new SettingsForm {Owner = this}.ShowDialog();
        }

        /// <summary>
        /// Обработчик таймера, вешаем всякие обработки формы.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerTick(object sender, EventArgs e)
        {
            var isEnabled = _loadThread == null || _loadThread.ThreadState == ThreadState.Stopped;
            this.TextBlock.Text = Library.Status;
            UpdateButton.IsEnabled = isEnabled;
            AddButton.IsEnabled = isEnabled;
            SettingsButton.IsEnabled = isEnabled;
            FormLibrary.IsEnabled = isEnabled;
        }

        /// <summary>
        /// Клик правой кнопкой.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormLibrary_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null || listBox.SelectedItems.Count == 0 || !(e.MouseDevice.DirectlyOver is Image))
                return;

            var manga = listBox.SelectedItem as Manga;
            if (manga == null)
                return;

            var openFolder = new MenuItem() {Header = Strings.Manga_Action_OpenFolder, FontWeight = FontWeights.Bold};
            openFolder.Click += (o, args) => MenuOpenFolder(manga);
            var update = new MenuItem() {Header = Strings.Manga_Action_Update};
            update.Click += (o, agrs) => UpdateManga(manga);
            var remove = new MenuItem() {Header = Strings.Manga_Action_Remove};
            remove.Click += (o, agrs) => Library.Remove(manga);
            var view = new MenuItem() {Header = Strings.Manga_Action_View};
            view.Click += (o, agrs) => Process.Start(manga.Url);

            var menu = new ContextMenu();
            menu.Items.Add(openFolder);
            menu.Items.Add(update);
            menu.Items.Add(view);
            menu.Items.Add(remove);
            this.FormLibrary.ContextMenu = menu;
        }

        private static void UpdateManga(Manga manga)
        {
            if (_loadThread == null || _loadThread.ThreadState == ThreadState.Stopped)
                _loadThread = new Thread(() => Library.Update(manga));
            if (_loadThread.ThreadState == ThreadState.Unstarted)
                _loadThread.Start();
        }

        private static void MenuOpenFolder(Manga manga)
        {
            if (Directory.Exists(manga.Folder))
                Process.Start(manga.Folder);
            else
                Library.Status = Strings.Library_Status_FolderNotFound;
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Settings.WindowsState = new object[]{this.Top, this.Left, this.Width, this.Height, this.WindowState};
        }

        private void FormLibrary_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null || listBox.SelectedItems.Count == 0)
                return;

            if (!(e.MouseDevice.DirectlyOver is Image))
                listBox.SelectedIndex = -1;
        }
    }
}