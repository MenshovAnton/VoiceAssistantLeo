﻿using System.Windows.Controls;
using System.Windows.Input;

namespace VA_Leo
{
    /// <summary>
    /// Логика взаимодействия для Chat.xaml
    /// </summary>
    public partial class Chat : Page
    {
        public Chat()
        {
            InitializeComponent();
        }

        private void sendButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Vosk vosk = new Vosk();
            Vosk.txt = TextBox.Text.ToLower();
            vosk.SpeechRecognized();
            TextBox.Text = string.Empty;
        }

        private void SendBtnMouseEnter(object sender, MouseEventArgs e)
        {
            SendButtonBackgound.Opacity = 0.2;
        }

        private void SendBtnMouseLeave(object sender, MouseEventArgs e)
        {
            SendButtonBackgound.Opacity = 0;
        }

        private void Chat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                sendButton_MouseDown(SendButton, null);
            }
        }
    }
}
