using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Leo.Classes;

namespace Leo.PageModels
{
    
    public partial class Chat
    {
        private static Chat? _instance;
        private static string _textMessage = string.Empty;
        public static bool NullMessages = true;
        private static ScrollViewer? _scrollViewer;
        private static Dispatcher? _dispatcher;
        
        public Chat()
        {
            InitializeComponent();
            TextBox.Text = _textMessage;
            ChatList.ItemsSource = Message.MessagesCollection;
            _instance = this;
            
            var currentDispatcher = Dispatcher.CurrentDispatcher;
            _dispatcher = currentDispatcher;

            if (!NullMessages)
            {
                HelloLabel.Visibility = Visibility.Hidden;
            }
            
            ScrollBox.ScrollToEnd();
            _scrollViewer = ScrollBox;

            TextBox.Focus();
        }
        
        public static void addMessageItem(string text, string alignment)
        {
            if (text == string.Empty)
            {
                return;
            }

            if (NullMessages)
            {
                NullMessages = false;
            }

            var ft = new FormattedText(text, 
                CultureInfo.CurrentCulture, 
                0, 
                new Typeface("Montserrat Alternates"), 
                14, 
                Brushes.White, 
                96);
            var length = (int)ft.WidthIncludingTrailingWhitespace + 20;

            if (_instance!.HelloLabel.Visibility == Visibility.Visible)
            {
                _dispatcher?.BeginInvoke(DispatcherPriority.Normal, () => _instance.HelloLabel.Visibility = Visibility.Hidden);
            }

            var isDateVisible = true;
            if (Properties.Settings.Default.nowDate == DateTime.Now.ToShortDateString())
            {
                isDateVisible = false;
            }
            else
            {
                Properties.Settings.Default.nowDate = DateTime.Now.ToShortDateString();
                Properties.Settings.Default.Save();
            }

            Message.addMessage(text, length, alignment, isDateVisible);
            
            _scrollViewer?.ScrollToEnd();
        }
        
        public static void addMessageItem(string? text, string? alignment, string? time, string? date, bool isDateVisible)
        {
            if (text == string.Empty)
            {
                return;
            }

            if (NullMessages)
            {
                NullMessages = false;
            }

            var ft = new FormattedText(text, 
                CultureInfo.CurrentCulture, 
                0, 
                new Typeface("Montserrat Alternates"), 
                14, 
                Brushes.White, 
                96);
            var length = (int)ft.WidthIncludingTrailingWhitespace + 20;

            if (_instance!.HelloLabel.Visibility == Visibility.Visible)
            {
                _instance.HelloLabel.Visibility = Visibility.Hidden;
            }
            
            Message.addMessage(text!, time!, length, alignment!, date!, isDateVisible);
            
            _scrollViewer?.ScrollToEnd();
        }

        private void send(object sender, MouseButtonEventArgs? e)
        {
            addMessageItem(TextBox.Text, "Right");
            
            var vosk = new VoskRecognizer();
            VoskRecognizer.RecognizedText = TextBox.Text.ToLower();
            vosk.speechRecognized();

            Console.WriteLine($@"[INPUT] Input > {VoskRecognizer.RecognizedText}");

            TextBox.Text = string.Empty;
            _textMessage = string.Empty;
        }

        private void sendBtnMouseEnter(object sender, MouseEventArgs e)
        {
            SendButtonBackground.Opacity = 0.2;
        }

        private void sendBtnMouseLeave(object sender, MouseEventArgs e)
        {
            SendButtonBackground.Opacity = 0;
        }

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
        {
            if (TextBox.Text != "")
            {
                _textMessage = TextBox.Text;
            }
        }
        
        public static void initialMessage(string message)
        {
            var recognizedText = VoskRecognizer.RecognizedText![..1].ToUpper() + (VoskRecognizer.RecognizedText.Length > 1 ? VoskRecognizer.RecognizedText[1..] : "");
            
            _dispatcher?.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                addMessageItem(recognizedText, "Right");
                addMessageItem(message, "Left");
            });
        }
    }
}
