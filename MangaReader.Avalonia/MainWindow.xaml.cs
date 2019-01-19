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
using MangaReader.Avalonia.Services;
using MangaReader.Avalonia.ViewModel;
using MangaReader.Core.Convertation;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;

namespace MangaReader.Avalonia
{
  public class MainWindow : Window
  {
    private ExplorerViewModel explorer = ExplorerViewModel.Instance;

    public MainWindow()
    {
      this.InitializeComponent();
      Title = "Loading...";
      ConfigStorage.Instance.ViewConfig.UpdateWindowState(this);
      App.AttachDevTools(this);
      MangaReader.Core.Update.Updater.NewVersionFound += UpdaterOnNewVersionFound;
      explorer.LoadingProcess.Status = Title;
      Task.Run(() => MangaReader.Core.Client.Start(explorer.LoadingProcess));
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
            var appliedUserControl = cp?.GetLogicalChildren().FirstOrDefault();
            var cpGrid = appliedUserControl?.GetLogicalChildren().OfType<Grid>().FirstOrDefault();
            var textBox = cpGrid?.GetLogicalChildren().OfType<TextBox>().FirstOrDefault();
            textBox?.Focus();
          }, DispatcherPriority.Background);
        });
    }

    private async void UpdaterOnNewVersionFound(object sender, string e)
    {
      var dialog = new Dialogs.Avalonia.Dialog
      {
        Title = $"Найдено обновление {e}",
        Description = "Автоматическое обновление не реализовано в текущей версии, обновитесь вручную."
      };
      var download = dialog.Buttons.AddButton("Скачать обновление");
      var close = dialog.Buttons.AddButton("Закрыть");

      await Dispatcher.UIThread.InvokeAsync(() =>
      {
        if (dialog.Show() == download)
        {
          Helper.StartUseShell(MangaReader.Core.Update.Updater.RepositoryReleaseUri);
        }
      }).LogException();
    }

    protected override bool HandleClosing()
    {
      ConfigStorage.Instance.ViewConfig.SaveWindowState(this);
      return base.HandleClosing();
    }

    private void InitializeComponent()
    {
      AvaloniaXamlLoader.Load(this);
    }
  }
}