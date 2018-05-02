using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MangaReader.Avalonia.View
{
    public class Library : UserControl
    {
        public Library()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
