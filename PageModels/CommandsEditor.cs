using System.Windows;
using System.Windows.Controls;
using Leo.WindowModels;

namespace Leo.PageModels;

public partial class CommandsEditor : Page
{
    public CommandsEditor()
    {
        InitializeComponent();
    }
    
    private void back(object sender, RoutedEventArgs e)
    {
        MainWindow.backPage();
    }
}