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
  public partial class Download : Window, INotifyPropertyChanged
  {
    public double ProgressPercentage
    {
      get { return progressPercentage; }
      set
      {
        if (progressPercentage < value)
        {
          progressPercentage = value;
          OnPropertyChanged();
        }
      }
    }

    private double progressPercentage;

    public string Text
    {
      get { return text; }
      set
      {
        text = value;
        OnPropertyChanged();
      }
    }

    private string text;

    private string DefaultText = "Скачивается обновление";

    public void UpdateStates(DownloadProgressChangedEventArgs args)
    {
      this.ProgressPercentage = args.ProgressPercentage;
      this.Text = string.Format("{0} - {1}/{2} МБ", this.DefaultText, args.BytesReceived.ToMegaBytes(), args.TotalBytesToReceive.ToMegaBytes());
    }
    
    public Download(Window owner)
    {
      InitializeComponent();
      this.Text = this.DefaultText + "...";
      this.DataContext = this;
      if (owner != null)
      {
        this.Owner = owner;
        this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}
