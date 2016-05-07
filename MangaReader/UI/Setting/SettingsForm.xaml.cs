using System.Windows;
using MangaReader.Services;

namespace MangaReader.UI.Setting
{
  /// <summary>
  /// Логика взаимодействия для SettingsForm.xaml
  /// </summary>
  public partial class SettingsForm : Window
  {
    public SettingsForm()
    {
      Owner = WindowHelper.Owner;
      InitializeComponent();
      ViewModel.Primitive.BaseViewModel.SubToViewModel(this);
    }
  }
}
