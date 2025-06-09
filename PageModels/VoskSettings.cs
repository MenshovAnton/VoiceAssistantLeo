using System.Windows;
using Leo.WindowModels;

namespace Leo.PageModels;

public partial class VoskSettings
{
    public VoskSettings()
    {
        InitializeComponent();
    }

    private void back(object sender, RoutedEventArgs e)
    {
        MainWindow.backPage();
    }
}