using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MangaReader.Core.Services;

namespace MangaReader.UI.LogViewer
{
  /// <summary>
  /// Interaction logic for LogViewer.xaml
  /// </summary>
  public partial class LogViewer : UserControl
  {
    public ObservableCollection<LogEventStruct> LogEntries { get; private set; }

    public LogViewer()
    {
      InitializeComponent();
      LogEntries = new ObservableCollection<LogEventStruct>();

      if (!DesignerProperties.GetIsInDesignMode(this))
      {
        Core.Services.Log.LogReceived += LogReceived;
      }
    }

    private void ResizeColumns()
    {
      var time = GridView.Columns[0].ActualWidth;
      var level = GridView.Columns[1].ActualWidth;
      var message = GridView.Columns[2].ActualWidth;
      var total = time + level + message;
      var actualTotal = GetPropertyByName(LogView, "ItemsHost")?.ActualWidth ?? LogView.ActualWidth;
      if (Math.Abs(total - actualTotal) > 0.1)
      {
        var newMessageWidth = actualTotal - time - level - 20;
        if (newMessageWidth > 0)
          GridView.Columns[2].Width = newMessageWidth;
      }
    }

    private static FrameworkElement GetPropertyByName(FrameworkElement element, string name)
    {
      return element.GetType().GetProperty(name, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(element, null) as FrameworkElement;
    }

    protected void LogReceived(LogEventStruct log)
    {
      var empty = !LogEntries.Any();
      Dispatcher.BeginInvoke(new Action(() =>
      {
        LogEntries.Add(log);
        LogView.SelectedIndex = LogView.Items.Count - 1;
        LogView.ScrollIntoView(LogView.SelectedItem);
        ResizeColumns();
      }));

      if (empty)
        Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(ResizeColumns));
    }

    private void ParentGrid_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
      ResizeColumns();
    }

    private void TextBlockWithLinks_OnHyperlinkPressed(object sender, HyperlinkPressedEventArgs args)
    {
      Helper.StartUseShell(args.Hyperlink.OriginalString);
    }
  }
}
