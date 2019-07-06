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
using MangaReader.Core;
using MangaReader.Core.Convertation;
using MangaReader.Core.Services;
using MangaReader.Core.Services.Config;
using MangaReader.Core.Update;

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
      Task.Run(() => Client.Start(explorer.LoadingProcess));
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
          }, DispatcherPriority.Background).ConfigureAwait(true);
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

      await Dispatcher.UIThread.InvokeAsync(async () =>
      {
        if (await dialog.ShowAsync().ConfigureAwait(true) == download)
        {
          Helper.StartUseShell(Updater.RepositoryReleaseUri);
        }
      }).LogException().ConfigureAwait(true);
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
