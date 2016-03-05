using System.Windows;

namespace MangaReader.Services
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
    }

    /// <summary>
    /// Показать историю, центрировав окно по родительскому.
    /// </summary>
    /// <param name="owner">Родительское окно истории изменений.</param>
    public VersionHistoryView(Window owner) : this()
    {
      this.Owner = owner;
      if (this.Owner != null)
        this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }
  }
}
