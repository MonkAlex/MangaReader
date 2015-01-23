using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace MangaReader.Services
{
  /// <summary>
  /// Логика взаимодействия для Converting.xaml
  /// </summary>
  public partial class Converting : Window
  {

    /// <summary>
    /// Таймер на обновление формы.
    /// </summary>
    // ReSharper disable once NotAccessedField.Local
    private static DispatcherTimer _timer;

    public Converting()
    {
      InitializeComponent();
      _timer = new DispatcherTimer(new TimeSpan(0, 0, 0, 0, 100),
        DispatcherPriority.Background,
        TimerTick,
        Dispatcher.CurrentDispatcher);
    }

    public void ShowDialog(Action action)
    {
      var thread = new Thread(() =>
      {
        action();
        this.Dispatcher.Invoke(this.Close, DispatcherPriority.Send);
      });
      thread.Start();
      Thread.Sleep(500);

      if (thread.ThreadState == ThreadState.Running)
        this.ShowDialog();
    }

    private void TimerTick(object o, EventArgs e)
    {
      var process = Converter.Process;
      if (process != null)
      {
        this.TextBlock.Text = process.Status;
        this.ProgressBar.IsIndeterminate = process.IsIndeterminate;
        this.ProgressBar.Value = process.Percent;
      }

      if (Converter.State == ConverterState.Completed)
        this.Close();
    }
  }
}
