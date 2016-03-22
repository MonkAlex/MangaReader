using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using MangaReader.Core.Services.Config;
using MangaReader.Manga;
using MangaReader.Properties;
using MangaReader.Services;
using MangaReader.UI.MainForm;
using MangaReader.ViewModel.Commands;

namespace MangaReader
{
  /// <summary>
  /// Логика взаимодействия для Table.xaml
  /// </summary>
  public partial class Table : BaseForm
  {
    /// <summary>
    /// Таймер на обновление формы.
    /// </summary>
    // ReSharper disable once NotAccessedField.Local
    private static DispatcherTimer _timer;

    public Table()
    {
      InitializeComponent();
      _timer = new DispatcherTimer(new TimeSpan(0, 0, 1),
          DispatcherPriority.Background,
          TimerTick,
          Dispatcher.CurrentDispatcher);
    }

    /// <summary>
    /// Обработчик двойного клика по манге.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Mangas_clicked(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount < 2)
        return;

      var item = sender as ListViewItem;
      if (item == null)
        return;

      var downloadable = item.DataContext as IDownloadable;
      if (downloadable == null)
        return;

      var defaultCommand = Model.MangaMenu.FirstOrDefault(m => m.IsDefault);
      if (defaultCommand != null && defaultCommand.Command.CanExecute(downloadable))
        defaultCommand.Command.Execute(downloadable);
    }

    /// <summary>
    /// Обработчик таймера, вешаем всякие обработки формы.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TimerTick(object sender, EventArgs e)
    {
      this.TextBlock.Text = Library.Status;

      if (Library.IsAvaible && ConfigStorage.Instance.AppConfig.AutoUpdateInHours > 0 &&
        DateTime.Now > ConfigStorage.Instance.AppConfig.LastUpdate.AddHours(ConfigStorage.Instance.AppConfig.AutoUpdateInHours))
      {
        Log.Add(Strings.AutoUpdate);
        new UpdateWithPauseCommand(Model.View).Execute(sender);
      }
    }

    private void ListView_MouseDown(object sender, MouseButtonEventArgs e)
    {
      var listView = sender as ListView;
      if (listView != null)
      {
        listView.SelectedIndex = -1;
      }
    }

    private void FilterChanged(object sender, RoutedEventArgs e)
    {
      if (Model.View != null)
      {
        Model.View.Refresh();
      }
    }
  }

  [ValueConversion(typeof(string), typeof(string))]
  public class UrlTypeConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      var result = value == null ? string.Empty : value.ToString();
      if (result.Contains("readmanga"))
        return "RM";
      if (result.Contains("adultmanga"))
        return "AM";
      if (result.Contains("acomics"))
        return "AC";
      if (result.Contains("hentaichan"))
        return "HC";
      if (result.Contains("mintmanga.com"))
        return "MM";
      return "NA";
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion
  }

  [ValueConversion(typeof(bool), typeof(string))]
  public class CompletedImageConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      var result = "Icons/play.png";
      switch ((bool)value)
      {
        case true:
          result = "Icons/stop.png";
          break;

        case false:
          result = "Icons/play.png";
          break;
      }
      return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion
  }

  [ValueConversion(typeof(bool), typeof(string))]
  public class UpdateImageConverter : IValueConverter
  {
    #region IValueConverter Members

    public object Convert(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      var result = "Icons/play.png";
      switch ((bool)value)
      {
        case true:
          result = "Icons/yes.png";
          break;

        case false:
          result = "Icons/no.png";
          break;
      }
      return result;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion
  }
}