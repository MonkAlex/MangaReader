using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MangaReader.UI
{
  /// <summary>
  /// Interaction logic for MultiSelectComboBox.xaml
  /// </summary>
  public partial class MultiSelectComboBox : UserControl
  {
    private ObservableCollection<Node> _nodeList;
    public MultiSelectComboBox()
    {
      InitializeComponent();
      _nodeList = new ObservableCollection<Node>();
    }

    #region Dependency Properties

    public static readonly DependencyProperty ItemsSourceProperty =
         DependencyProperty.Register("ItemsSource", typeof(Dictionary<string, object>), typeof(MultiSelectComboBox), new FrameworkPropertyMetadata(null,
    new PropertyChangedCallback(OnItemsSourceChanged)));

    public static readonly DependencyProperty SelectedItemsProperty =
     DependencyProperty.Register("SelectedItems", typeof(Dictionary<string, object>), typeof(MultiSelectComboBox), new FrameworkPropertyMetadata(null,
 new PropertyChangedCallback(OnSelectedItemsChanged)));

    public static readonly DependencyProperty TextProperty =
       DependencyProperty.Register("Text", typeof(string), typeof(MultiSelectComboBox), new UIPropertyMetadata(string.Empty));

    public static readonly DependencyProperty DefaultTextProperty =
        DependencyProperty.Register("DefaultText", typeof(string), typeof(MultiSelectComboBox), new UIPropertyMetadata(string.Empty));

    private static readonly string _All = "All";


    public Dictionary<string, object> ItemsSource
    {
      get { return (Dictionary<string, object>)GetValue(ItemsSourceProperty); }
      set
      {
        SetValue(ItemsSourceProperty, value);
      }
    }

    public Dictionary<string, object> SelectedItems
    {
      get { return (Dictionary<string, object>)GetValue(SelectedItemsProperty); }
      set
      {
        SetValue(SelectedItemsProperty, value);
      }
    }

    public string Text
    {
      get { return (string)GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
    }

    public string DefaultText
    {
      get { return (string)GetValue(DefaultTextProperty); }
      set { SetValue(DefaultTextProperty, value); }
    }
    #endregion

    #region Events
    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = (MultiSelectComboBox)d;
      control.DisplayInControl();
    }

    private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = (MultiSelectComboBox)d;
      control.SelectNodes();
      control.SetText();
    }

    private void CheckBox_Click(object sender, RoutedEventArgs e)
    {
      var clickedBox = (CheckBox)sender;

      if (clickedBox.Content.ToString() == _All)
      {
        foreach (Node node in _nodeList)
        {
          node.IsSelected = clickedBox.IsChecked.Value;
        }
      }
      else
      {
        _nodeList.Single(i => i.Title == _All).IsSelected = _nodeList.Count(s => s.IsSelected && s.Title != _All) == _nodeList.Count - 1;
      }
      SetSelectedItems();
      SetText();

    }
    #endregion


    #region Methods
    private void SelectNodes()
    {
      foreach (var node in SelectedItems.Select(keyValue => _nodeList.FirstOrDefault(i => i.Title == keyValue.Key)).Where(node => node != null))
      {
        node.IsSelected = true;
      }
    }

    private void SetSelectedItems()
    {
      if (SelectedItems == null)
        SelectedItems = new Dictionary<string, object>();
      SelectedItems.Clear();
      foreach (var node in _nodeList.Where(node => node.IsSelected && node.Title != _All && this.ItemsSource.Count > 0))
      {
        SelectedItems.Add(node.Title, this.ItemsSource[node.Title]);
      }
    }

    private void DisplayInControl()
    {
      _nodeList.Clear();
      if (this.ItemsSource.Count > 0)
        _nodeList.Add(new Node(_All));
      foreach (var node in this.ItemsSource.Select(keyValue => new Node(keyValue.Key)))
      {
        _nodeList.Add(node);
      }
      MultiSelectCombo.ItemsSource = _nodeList;
    }

    private void SetText()
    {
      if (this.SelectedItems != null)
      {
        var displayText = new StringBuilder();
        foreach (var s in _nodeList)
        {
          if (s.IsSelected == true && s.Title == _All)
          {
            displayText = new StringBuilder();
            displayText.Append(_All);
            break;
          }
          else if (s.IsSelected == true && s.Title != _All)
          {
            displayText.Append(s.Title);
            displayText.Append(',');
          }
        }
        this.Text = displayText.ToString().TrimEnd(',');
      }
      // set DefaultText if nothing else selected
      if (string.IsNullOrEmpty(this.Text))
      {
        this.Text = this.DefaultText;
      }
    }


    #endregion
  }

  public class Node : INotifyPropertyChanged
  {

    private string _title;
    private bool _isSelected;
    #region ctor
    public Node(string title)
    {
      Title = title;
    }
    #endregion

    #region Properties
    public string Title
    {
      get
      {
        return _title;
      }
      set
      {
        _title = value;
        NotifyPropertyChanged("Title");
      }
    }
    public bool IsSelected
    {
      get
      {
        return _isSelected;
      }
      set
      {
        _isSelected = value;
        NotifyPropertyChanged("IsSelected");
      }
    }
    #endregion

    public event PropertyChangedEventHandler PropertyChanged;
    protected void NotifyPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

  }
}
