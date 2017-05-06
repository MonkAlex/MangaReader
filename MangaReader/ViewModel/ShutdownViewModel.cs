using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Shell;
using System.Windows.Threading;
using MangaReader.Core.Convertation;
using MangaReader.Services;
using MangaReader.UI.Services;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel
{
  public class ShutdownViewModel : ProcessModel, IDisposable
  {
    private const int Period = 1000;
    private long seconds;
    private readonly Timer timer;
    private string titleText;
    private string text;

    private static void TimerCallback(object state)
    {
      var model = state as ShutdownViewModel;
      if (model == null)
        return;

      if (model.CancellationToken.IsCancellationRequested)
      {
        model.timer.Change(Timeout.Infinite, Timeout.Infinite);
        Client.Dispatcher.Invoke(() =>
        {
          var window = ViewService.Instance.TryGet<System.Windows.Window>(model);
          if (window != null && window.IsVisible)
            window.Close();
        });
        return;
      }

      model.Seconds--;
      model.TitleText = $"{model.Seconds} сек до выключения...";
      model.Text = $"Через {model.Seconds} сек компьютер будет выключен.";

      if (model.Seconds <= 0 && !model.CancellationToken.IsCancellationRequested)
      {
        model.CancellationToken.Cancel();
        Client.Dispatcher.Invoke(() =>
        {
          Process.Start(new ProcessStartInfo("shutdown", "/s /t 0")
          {
            CreateNoWindow = true,
            UseShellExecute = false
          });
        });
      }
    }

    public CancellationTokenSource CancellationToken { get; }

    public long Seconds
    {
      get { return seconds; }
      set
      {
        seconds = value;
        Percent = (MaxValue - value) * 1.00 / MaxValue;
        OnPropertyChanged();
      }
    }

    public long MaxValue { get; }

    public string TitleText
    {
      get { return titleText; }
      set
      {
        titleText = value;
        OnPropertyChanged();
      }
    }

    public string Text
    {
      get { return text; }
      set
      {
        text = value;
        OnPropertyChanged();
      }
    }

    public override void Show()
    {
      base.Show();

      var window = ViewService.Instance.TryGet<Window>(this);
      if (window != null)
      {
        window.Closing += (o, a) =>
        {
          CancellationToken.Cancel();
          this.Dispose();
        };
        timer.Change(0, Period);
        ProgressState = ProgressState.Error;
        window.Topmost = true;
        window.Show();
      }
    }

    public ShutdownViewModel()
    {
      CancellationToken = new CancellationTokenSource();
      timer = new Timer(TimerCallback, this, Timeout.Infinite, Period);
      MaxValue = 31;
      Seconds = MaxValue;
    }

    public void Dispose()
    {
      timer?.Dispose();
      CancellationToken?.Dispose();
    }
  }
}
