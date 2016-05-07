using System.Windows;
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
      InitializeComponent();
      ViewModel.Primitive.BaseViewModel.SubToViewModel(this);
      this.Owner = WindowHelper.Owner;
      if (this.Owner != null)
        this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }
  }
}
