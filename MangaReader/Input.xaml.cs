using System.Windows;

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
