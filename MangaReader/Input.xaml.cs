using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MangaReader.Services;
using MangaReader.UI;

namespace MangaReader
{
  /// <summary>
  /// Логика взаимодействия для Input.xaml
  /// </summary>
  public partial class Input : Window
  {
    public Input()
    {
      InitializeComponent();
      Result.Focus();
      foreach (var setting in Settings.MangaSettings)
      {
        this.BookmarksTabs.Items.Add(new TabItem() {Content = new Login(setting), Header = setting.MangaName});
      }
    }

    public string ResponseText
    {
      get { return Result.Text; }
      set { Result.Text = value; }
    }

    private void Add_click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
      this.Close();
    }
  }
}
