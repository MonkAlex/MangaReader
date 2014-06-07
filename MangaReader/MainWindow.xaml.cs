using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MangaReader.Properties;
using MangaReader.Services;
using Ookii.Dialogs.Wpf;
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
            this.FormLibrary.ItemsSource = Library.Initialize();
            Convert();
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

            TaskDialogButton result;
            using (var dialog = new TaskDialog())
            {
                dialog.WindowTitle = manga.Name;
                dialog.Content = manga.Status;

                dialog.Buttons.Add(new TaskDialogButton(Strings.Manga_Action_Update));
                dialog.Buttons.Add(new TaskDialogButton(Strings.Manga_Action_Remove));
                dialog.Buttons.Add(new TaskDialogButton(Strings.Manga_Action_View));
                dialog.Buttons.Add(new TaskDialogButton { ButtonType = ButtonType.Cancel });

                dialog.AllowDialogCancellation = true;
                result = dialog.ShowDialog(this);
            }

            if (result.Text == Strings.Manga_Action_Update)
            {
                if (_loadThread == null || _loadThread.ThreadState == ThreadState.Stopped)
                    _loadThread = new Thread(() => Library.Update(manga));
                if (_loadThread.ThreadState == ThreadState.Unstarted)
                    _loadThread.Start();
            }
            if (result.Text == Strings.Manga_Action_Remove)
                Library.Remove(manga);
            if (result.Text == Strings.Manga_Action_View)
                Process.Start(manga.Url);
        }

        /// <summary>
        /// Добавление манги. Используем кастомный диалог.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_click(object sender, RoutedEventArgs e)
        {
            var db = new Input { Owner = this };
            if (db.ShowDialog() == true)
                Library.Add(db.Result.Text);
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
    }
}