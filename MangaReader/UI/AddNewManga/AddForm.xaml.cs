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

namespace MangaReader.UI.AddNewManga
{
  /// <summary>
  /// Interaction logic for AddForm.xaml
  /// </summary>
  public partial class AddForm : Window
  {
    public AddForm()
    {
      InitializeComponent();
      Owner = WindowHelper.Owner;
      ViewModel.Primitive.BaseViewModel.SubToViewModel(this);
    }
  }
}
