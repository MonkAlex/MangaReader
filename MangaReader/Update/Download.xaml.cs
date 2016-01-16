using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using MangaReader.Services;

namespace MangaReader.Update
{
  /// <summary>
  /// Interaction logic for Download.xaml
  /// </summary>
  public partial class Download : Window
  {
    public Download(Window owner)
    {
      InitializeComponent();
      if (owner != null)
      {
        this.Owner = owner;
        this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      }
    }
  }
}
