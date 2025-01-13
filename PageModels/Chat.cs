using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Leo.Classes;
using Leo.WindowModels;

namespace Leo.PageModels
{
    public partial class Chat
    {
        private static Chat? _chat;
        private static string _textMessage = string.Empty;
        public static bool NullMessages = true;
        private static ScrollViewer? _scrollViewer;

        private static readonly ChatManager ChatManager = new();
        
        public Chat()
        {
            InitializeComponent();
            TextBox.Text = _textMessage;
            ChatList.ItemsSource = MainWindow.ChatCollection;
            _chat = this;

            if (!NullMessages) 
            { HelloLabel.Visibility = Visibility.Hidden; }
            
            ScrollBox.ScrollToEnd();
            _scrollViewer = ScrollBox;

            TextBox.Focus();
        }

        public class Messages
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Global
            public string? Message { get; set; }
            public string? Time { get; set; }
            public int Length { get; set; }
            public string? Alignment { get; set; }
            public string? DateMessage { get; set; }
            public bool IsDateVisible { get; set; }
            // ReSharper restore UnusedAutoPropertyAccessor.Global
        }
        
        public static void addMessage(string text, string alignment)
        {
            if (text == string.Empty) { return; }
            if (NullMessages) { NullMessages = false; }

            var ft = new FormattedText(text, 
                CultureInfo.CurrentCulture, 
                0, 
                new Typeface("Montserrat Alternates"), 
                14, 
                Brushes.White, 
                96);
            var length = (int)ft.WidthIncludingTrailingWhitespace + 20;

            if (_chat!.HelloLabel.Visibility == Visibility.Visible)
            { _chat.HelloLabel.Visibility = Visibility.Hidden; }

            var isDateVisible = true;
            if (Properties.Settings.Default.nowDate == DateTime.Now.ToShortDateString())
            { isDateVisible = false; }
            else
            {
                Properties.Settings.Default.nowDate = DateTime.Now.ToShortDateString();
                Properties.Settings.Default.Save();
            }

            MainWindow.ChatCollection!.Add(new Messages
            {
                Message = text,
                Time = DateTime.Now.ToShortTimeString(),
                Length = length,
                Alignment = alignment,
                DateMessage = DateTime.Now.ToShortDateString(),
                IsDateVisible = isDateVisible
            });

            Properties.Settings.Default.messagesId += 1;
            Properties.Settings.Default.Save();
            
            ChatManager.serializeChat(text, alignment, DateTime.Now.ToShortTimeString(), 
                DateTime.Now.ToShortDateString(), isDateVisible, Properties.Settings.Default.messagesId);
            _scrollViewer?.ScrollToEnd();
        }
        
        public static void addMessage(string? text, string? alignment, string? time, string? date, bool isDateVisible)
        {
            if (text == string.Empty) { return; }
            if (NullMessages) { NullMessages = false; }

            var ft = new FormattedText(text, 
                CultureInfo.CurrentCulture, 
                0, 
                new Typeface("Montserrat Alternates"), 
                14, 
                Brushes.White, 
                96);
            var length = (int)ft.WidthIncludingTrailingWhitespace + 20;

            if (_chat!.HelloLabel.Visibility == Visibility.Visible) 
            { _chat.HelloLabel.Visibility = Visibility.Hidden; }

            MainWindow.ChatCollection!.Add(new Messages
            {
                Message = text,
                Time = time,
                Length = length,
                Alignment = alignment,
                DateMessage = date,
                IsDateVisible = isDateVisible
            });
            
            _scrollViewer?.ScrollToEnd();
        }

        private void send(object sender, MouseButtonEventArgs? e)
        {
            addMessage(TextBox.Text, "Right");
            
            var vosk = new Classes.Vosk();
            Classes.Vosk.RecognizedText = TextBox.Text.ToLower();
            vosk.speechRecognized();

            Console.WriteLine($@"[INPUT] Input > {Classes.Vosk.RecognizedText}");

            TextBox.Text = string.Empty;
            _textMessage = string.Empty;
        }

        private void sendBtnMouseEnter(object sender, MouseEventArgs e)
        { SendButtonBackground.Opacity = 0.2; }

        private void sendBtnMouseLeave(object sender, MouseEventArgs e)
        { SendButtonBackground.Opacity = 0; }

        private void hotKeys(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    send(SendButton, null);
                    break;
                case Key.Up:
                    if (_textMessage != string.Empty)
                        TextBox.Text = _textMessage;
                    else
                        System.Media.SystemSounds.Exclamation.Play();
                    break;
            }
        }

        private void messageBuffer(object sender, TextChangedEventArgs e)
        { if (TextBox.Text != "") { _textMessage = TextBox.Text; } }
    }
}
