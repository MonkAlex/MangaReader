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
        private static DispatcherTimer timer;

        /// <summary>
        /// Поток загрузки манги.
        /// </summary>
        private static Thread loadThread;

        public MainWindow()
        {
            Update.Initialize();
            InitializeComponent();
            Initialize();
            this.FormLibrary.ItemsSource = Library.Initialize();
        }

        public void Initialize()
        {
            timer = new DispatcherTimer(new TimeSpan(0, 0, 1),
                DispatcherPriority.Background,
                TimerTick,
                Dispatcher.CurrentDispatcher);
        }

        private void Update_click(object sender, RoutedEventArgs e)
        {
            if (loadThread == null || loadThread.ThreadState == ThreadState.Stopped)
                loadThread = new Thread(() => Library.Update());
            if (loadThread.ThreadState == ThreadState.Unstarted)
                loadThread.Start();
        }

        private void Mangas_clicked(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null || listBox.SelectedItems.Count == 0)
                return;

            var manga = listBox.SelectedItem as Manga;
            if (manga == null)
                return;

            if (loadThread == null || loadThread.ThreadState == ThreadState.Stopped)
                loadThread = new Thread(() => Library.Update(manga, true));
            if (loadThread.ThreadState == ThreadState.Unstarted)
                loadThread.Start();
        }

        private void Add_click(object sender, RoutedEventArgs e)
        {
            var db = new Input { Owner = this };
            if (db.ShowDialog() == true)
                Library.Add(db.Result.Text);
        }

        private void TimerTick(object sender, EventArgs e)
        {
            var isEnabled = loadThread == null || loadThread.ThreadState == ThreadState.Stopped;
            this.TextBlock.Text = Library.Status;
            UpdateButton.IsEnabled = isEnabled;
            AddButton.IsEnabled = isEnabled;
            FormLibrary.IsEnabled = isEnabled;
        }
    }
}