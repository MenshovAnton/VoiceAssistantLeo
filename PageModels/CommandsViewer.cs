using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Leo.Classes;
using Leo.WindowModels;

namespace Leo.PageModels
{
    public partial class CommandsViewer : Page
    {
        public CommandsViewer()
        {
            InitializeComponent();
            CommandsList.ItemsSource = Classes.Vosk.Commands;
        }

        private void getCommandsEditor(object sender, RoutedEventArgs args)
        {
            MainWindow.getCommandsEditorPage();
        }
    }
}