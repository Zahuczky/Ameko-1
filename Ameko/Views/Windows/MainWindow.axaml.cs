using Ameko.Views.Windows;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Ameko.Views.Windows;

public partial class MainWindow : Window
{
    private void ShowAboutDialog(object sender, RoutedEventArgs e)
    {
        var aboutBox = new AboutWindow();
        aboutBox.ShowDialog(this);
    }

    public MainWindow()
    {
        InitializeComponent();
    }
}
