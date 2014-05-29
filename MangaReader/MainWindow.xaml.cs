using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using MangaReader.Services;

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
            Update.Initialize();
            InitializeComponent();
            Initialize();
        }

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

        private void Update_click(object sender, RoutedEventArgs e)
        {
            if (_loadThread == null || _loadThread.ThreadState == ThreadState.Stopped)
                _loadThread = new Thread(() => Library.Update());
            if (_loadThread.ThreadState == ThreadState.Unstarted)
                _loadThread.Start();
        }

        private void Mangas_clicked(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null || listBox.SelectedItems.Count == 0 || !(e.MouseDevice.DirectlyOver is Image))
                return;

            var manga = listBox.SelectedItem as Manga;
            if (manga == null)
                return;

            if (_loadThread == null || _loadThread.ThreadState == ThreadState.Stopped)
                _loadThread = new Thread(() => Library.Update(manga, true));
            if (_loadThread.ThreadState == ThreadState.Unstarted)
                _loadThread.Start();
        }

        private void Add_click(object sender, RoutedEventArgs e)
        {
            var db = new Input { Owner = this };
            if (db.ShowDialog() == true)
                Library.Add(db.Result.Text);
        }

        private void Remove_click(object sender, RoutedEventArgs e)
        {
            var manga = this.FormLibrary.SelectedItem as Manga;
            if (manga == null)
                return;

            var message = MessageBox.Show("U want to remove " + manga.Name + "?", 
                manga.Name, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

            if (message == MessageBoxResult.Yes)
              Library.Remove(manga);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var isEnabled = _loadThread == null || _loadThread.ThreadState == ThreadState.Stopped;
            this.TextBlock.Text = Library.Status;
            UpdateButton.IsEnabled = isEnabled;
            AddButton.IsEnabled = isEnabled;
            RemoveButton.IsEnabled = isEnabled;
            FormLibrary.IsEnabled = isEnabled;
        }
    }
}