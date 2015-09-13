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
using MangaReader.Services;

namespace MangaReader.UI.MainForm
{
  /// <summary>
  /// Interaction logic for Blazard.xaml
  /// </summary>
  public partial class Blazard : BaseForm
  {

    public Blazard()
    {
      InitializeComponent();
      this.BlazardManga.DataContext = Library.SelectedManga;
    }

    private void FilterChanged(object sender, RoutedEventArgs e)
    {
      if (View != null)
      {
        View.Refresh();
      }
    }
  }
}
