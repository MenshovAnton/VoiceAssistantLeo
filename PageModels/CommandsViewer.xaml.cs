using System.Collections.ObjectModel;
using System.Windows.Controls;
using Leo.Classes;

namespace Leo.PageModels
{
    public partial class CommandsViewer : Page
    {
        public CommandsViewer()
        {
            InitializeComponent();
            CommandsList.ItemsSource = Classes.Vosk.Commands;
        }
    }
}