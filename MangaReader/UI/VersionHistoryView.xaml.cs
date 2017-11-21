using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using MangaReader.Services;

namespace MangaReader.UI
{
  /// <summary>
  /// Логика взаимодействия для VersionHistory.xaml
  /// </summary>
  public partial class VersionHistoryView : Window
  {
    /// <summary>
    /// Показать историю, центрировав окно по экрану.
    /// </summary>
    public VersionHistoryView()
    {
      ViewModel.Primitive.BaseViewModel.SubToViewModel(this);
      this.Owner = WindowHelper.Owner;
      InitializeComponent();
      if (this.Owner != null)
        this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }
  }

  public class TextToFlowDocumentConverter : DependencyObject, IValueConverter
  {
    public Markdown.Xaml.Markdown Markdown
    {
      get { return (Markdown.Xaml.Markdown)GetValue(MarkdownProperty); }
      set { SetValue(MarkdownProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Markdown.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MarkdownProperty =
        DependencyProperty.Register("Markdown", typeof(Markdown.Xaml.Markdown), typeof(TextToFlowDocumentConverter), new PropertyMetadata(null));

    /// <summary>
    /// Converts a value. 
    /// </summary>
    /// <returns>
    /// A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
      {
        return null;
      }

      var text = (string)value;

      var engine = Markdown ?? mMarkdown.Value;

      return engine.Transform(text);
    }

    /// <summary>
    /// Converts a value. 
    /// </summary>
    /// <returns>
    /// A converted value. If the method returns null, the valid null value is used.
    /// </returns>
    /// <param name="value">The value that is produced by the binding target.</param>
    /// <param name="targetType">The type to convert to.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    private Lazy<Markdown.Xaml.Markdown> mMarkdown
        = new Lazy<Markdown.Xaml.Markdown>(() => new Markdown.Xaml.Markdown());
  }

}
