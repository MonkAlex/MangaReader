using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using MangaReader.Core.Convertation;
using MangaReader.Core.Manga;
using MangaReader.Core.Services.Config;

namespace MangaReader.Avalonia
{
  public class MainWindow : Window
  {
    public MainWindow()
    {
      this.InitializeComponent();
      App.AttachDevTools(this);
      MangaReader.Core.Client.Init();
      var processTest = new ProcessTest();
      processTest.StateChanged += ProcessTestOnStateChanged;
      Task.Run(() => MangaReader.Core.Client.Start(processTest));
    }

    private void ProcessTestOnStateChanged(object sender, ConvertState convertState)
    {
      if (convertState == ConvertState.Completed)
      {
          Dispatcher.UIThread.InvokeAsync(() =>
          {
            this.DataContext = MangaReader.Core.NHibernate.Repository.Get<IManga>().ToList();
          }, DispatcherPriority.ApplicationIdle);
      }
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }

    private class ProcessTest : IProcess
    {
      private double percent;
      private ProgressState progressState;
      private string status;
      private Version version;
      private ConvertState state;

      public double Percent
      {
        get { return percent; }
        set { percent = value; }
      }

      public ProgressState ProgressState
      {
        get { return progressState; }
        set { progressState = value; }
      }

      public string Status
      {
        get { return status; }
        set { status = value; }
      }

      public Version Version
      {
        get { return version; }
        set { version = value; }
      }

      public ConvertState State
      {
        get { return state; }
        set
        {
          if (!Equals(state, value))
          {
            state = value;
            OnStateChanged(value);
          }
        }
      }

      public event EventHandler<ConvertState> StateChanged;

      public ProcessTest()
      {
        Version = AppConfig.Version;
      }

      protected virtual void OnStateChanged(ConvertState e)
      {
        StateChanged?.Invoke(this, e);
      }
    }
  }
}
