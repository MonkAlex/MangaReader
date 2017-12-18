using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MangaReader.ViewModel.Primitive;

namespace MangaReader.ViewModel.Commands.Primitives
{
  public class ContentMenuItem : Primitive.NotifyPropertyChanged
  {
    private string name;
    private ICommand command;
    private ObservableCollection<ContentMenuItem> subItems;
    private FontWeight fontWeight;
    private bool isDefault;
    private BaseCommand baseCommand;
    private ImageSourceVM icon;
    private bool isVisible;

    public FontWeight FontWeight
    {
      get { return fontWeight; }
      set
      {
        fontWeight = value;
        OnPropertyChanged();
      }
    }

    public bool IsDefault
    {
      get { return isDefault; }
      set
      {
        isDefault = value;
        this.FontWeight = value ? FontWeights.Bold : FontWeights.Normal;
        OnPropertyChanged();
      }
    }

    public string Name
    {
      get { return name; }
      set
      {
        name = value;
        OnPropertyChanged();
      }
    }

    public ImageSourceVM Icon
    {
      get { return icon; }
      set
      {
        icon = value;
        OnPropertyChanged();
      }
    }

    public ICommand Command
    {
      get { return command; }
      set
      {
        command = value;
        OnPropertyChanged();
      }
    }

    public bool IsVisible
    {
      get { return isVisible; }
      set
      {
        isVisible = value;
        OnPropertyChanged();
      }
    }

    public ObservableCollection<ContentMenuItem> SubItems
    {
      get { return subItems; }
      set
      {
        subItems = value;
        OnPropertyChanged();
      }
    }

    public ContentMenuItem(BaseCommand command) : this(command, command.Name)
    {
      this.baseCommand = command;
      this.Icon = new ImageSourceVM(command.Icon);
      this.baseCommand.PropertyChanged += CommandOnPropertyChanged;
    }

    private void CommandOnPropertyChanged(object sender, PropertyChangedEventArgs args)
    {
      if (args.PropertyName == nameof(baseCommand.Name))
        this.Name = baseCommand.Name;
      if (args.PropertyName == nameof(baseCommand.Icon))
        this.Icon = new ImageSourceVM(baseCommand.Icon);
      if (args.PropertyName == nameof(MultipleMangasBaseCommand.IsVisible))
        this.IsVisible = ((MultipleMangasBaseCommand) baseCommand).IsVisible;
    }

    public ContentMenuItem(ICommand command, string name) : this(name)
    {
      this.Command = command;
    }

    public ContentMenuItem(string name)
    {
      this.Name = name;
      this.IsVisible = true;
      this.SubItems = new ObservableCollection<ContentMenuItem>();
    }
  }
}