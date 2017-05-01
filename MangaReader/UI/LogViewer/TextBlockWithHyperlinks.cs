using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace MangaReader.UI.LogViewer
{
  public class TextBlockWithHyperlinks : TextBlock
  {
    private static readonly Regex HyperlinkRegex = new Regex(@"(https?|ftp):\/\/[^\s/$.?#].[^\s,]*");

    static TextBlockWithHyperlinks()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBlockWithHyperlinks), new FrameworkPropertyMetadata(typeof(TextBlockWithHyperlinks)));
    }

    public TextBlockWithHyperlinks()
    {
      TargetUpdated += (s, args) => Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle, new Action(ParseHyperlinks));
    }

    private void ParseHyperlinks()
    {
      var style = HyperlinkStyle;
      var text = Text;
      var matches = HyperlinkRegex.Matches(text);
      if (matches.Count == 0)
        return;

      Inlines.Clear();
      var lastIndex = 0;
      foreach (Match match in matches)
      {
        Inlines.Add(text.Substring(lastIndex, match.Index - lastIndex));
        lastIndex = match.Index + match.Length;
        var run = new Run(match.Value) { Style = style };
        run.MouseDown += RunOnMouseDown;
        Inlines.Add(run);
      }
      Inlines.Add(text.Substring(lastIndex));
    }

    private void RunOnMouseDown(object sender, MouseButtonEventArgs args)
    {
      var run = sender as Run;
      if (run == null)
        return;

      OnHyperlinkPressed(new HyperlinkPressedEventArgs(run.Text));
    }

    public event EventHandler<HyperlinkPressedEventArgs> HyperlinkPressed;

    public void OnHyperlinkPressed(HyperlinkPressedEventArgs args)
    {
      var handler = HyperlinkPressed;
      handler?.Invoke(this, args);
    }

    public static readonly DependencyProperty HyperlinkStyleProperty = DependencyProperty.Register("HyperlinkStyle",
                                                                                              typeof(Style),
                                                                                              typeof(TextBlockWithHyperlinks));

    public Style HyperlinkStyle
    {
      get { return (Style)GetValue(HyperlinkStyleProperty); }
      set { SetValue(HyperlinkStyleProperty, value); }
    }
  }

  public class HyperlinkPressedEventArgs : EventArgs
  {
    public readonly Uri Hyperlink;

    public HyperlinkPressedEventArgs(Uri hyperlink)
    {
      Hyperlink = hyperlink;
    }

    public HyperlinkPressedEventArgs(string hyperlink) : this(new Uri(hyperlink))
    {
    }
  }
}
