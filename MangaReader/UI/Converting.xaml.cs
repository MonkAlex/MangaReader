using System.Windows;
using MangaReader.Core.Services;

namespace MangaReader.Services
{
  /// <summary>
  /// Логика взаимодействия для Converting.xaml
  /// </summary>
  public partial class Converting : Window
  {
    public Converting()
    {
      InitializeComponent();
    }

    public Converting(Window owner) : this()
    {
      if (owner != null)
      {
        this.Owner = owner;
        this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      }
    }

    private void Converting_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      var oldValue = e.OldValue as IProcess;
      if (oldValue != null)
        oldValue.StateChanged -= OnStateChanged;

      var value = e.NewValue as IProcess;
      if (value != null)
        value.StateChanged += OnStateChanged;
    }

    private void OnStateChanged(object sender, ConvertState convertState)
    {
      if (convertState != ConvertState.Completed)
        return;

      Close();
    }
  }
}
