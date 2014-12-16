using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MangaReader.Manga
{
  /// <summary>
  /// Логика взаимодействия для MangaForm.xaml
  /// </summary>
  public partial class MangaForm : Window
  {
    public MangaForm()
    {
      InitializeComponent();
    }

    private void MangaForm_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var manga = e.NewValue as Mangas;
      if (manga == null)
        return;

      this.Title = manga.Name;
    }

    private void OkButton_OnClick(object sender, RoutedEventArgs e)
    {
      var manga = this.DataContext as Mangas;
      try
      {
        if (manga != null)
          manga.Save();
        this.Close();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
      var manga = this.DataContext as Mangas;
      if (manga != null)
        manga.Update();
      this.Close();
    }
  }

  [ValueConversion(typeof(bool), typeof(bool))]
  public class InverseBooleanConverter : IValueConverter
  {

    public object Convert(object value, Type targetType, object parameter,
      System.Globalization.CultureInfo culture)
    {
      return !(bool)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter,
        System.Globalization.CultureInfo culture)
    {
      throw new NotSupportedException();
    }

  }
}
