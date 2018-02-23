using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Avalonia.VisualTree;
using MangaReader.Avalonia.ViewModel;
using MangaReader.Core.Convertation;
using MangaReader.Core.Services.Config;

namespace MangaReader.Avalonia
{
  public class MainWindow : Window
  {
    private ExplorerViewModel explorer = ExplorerViewModel.Instance;

    public MainWindow()
    {
      this.InitializeComponent();
      App.AttachDevTools(this);
      MangaReader.Core.Client.Init();
      var processTest = new ProcessTest();
      processTest.StateChanged += ProcessTestOnStateChanged;
      Task.Run(() => MangaReader.Core.Client.Start(processTest));
      this.DataContext = explorer;

      // Focus to first textbox
      explorer.Changed
        .Where(c => c.PropertyName == nameof(ExplorerViewModel.SelectedTab))
        .Subscribe(async args =>
        {
          await Dispatcher.UIThread.InvokeAsync(() =>
          {
            var grid = this.LogicalChildren.OfType<Grid>().FirstOrDefault();
            var cp = grid?.GetLogicalChildren().OfType<ContentPresenter>().FirstOrDefault();
            var cpGrid = cp?.GetLogicalChildren().OfType<Grid>().FirstOrDefault();
            var textBox = cpGrid?.GetLogicalChildren().OfType<TextBox>().FirstOrDefault();
            textBox?.Focus();
          }, DispatcherPriority.Background);
        });
    }

    private void ProcessTestOnStateChanged(object sender, ConvertState convertState)
    {
      if (convertState == ConvertState.Completed)
      {
        explorer.SelectDefaultTab();
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