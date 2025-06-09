using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Leo.PageModels;
using Leo.Properties;
using Leo.WindowModels;
using NAudio.Wave;
using Newtonsoft.Json.Linq;
using Vosk;
using MessageBox = Leo.WindowModels.MessageBox;

namespace Leo.Classes
{ 
    public class VoskRecognizer
    {
        private static Dispatcher? _dispatcher;

        public static Vosk.VoskRecognizer? Recognizer; // Объект распознавания VOSK
        private static WaveFileWriter? _writer; // Объект записи с микрофона
        public static bool Busy;

        public static string? RecognizedText;
        private static bool _wakeWordStatus;
        
        private static readonly Logger Logger = new();
        private static readonly MessageBox MessageBox = new();
        public static readonly Stopwatch WakeTimer = new();
        private static readonly WaveInEvent WaveIn = new();
        
        private enum RecycleFlags : uint;

        public static void main()
        {
            var model = new Model(".\\VoskModel");
            Recognizer = new Vosk.VoskRecognizer(model, 16000f);
            
            WaveIn.WaveFormat = new WaveFormat(16000, 1);
            WaveIn.DataAvailable += WaveInOnDataAvailable;
            
            _writer = new WaveFileWriter(".\\voice.wav", WaveIn.WaveFormat);

            var currentDispatcher = Dispatcher.CurrentDispatcher;
            _dispatcher = currentDispatcher;
        }

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        static extern uint SHEmptyRecycleBin(IntPtr hwnd, string? pszRootPath, RecycleFlags dwFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32", SetLastError = true)]
        private static extern int GetWindowThreadProcessId([In] IntPtr hwnd, [Out] out int lProcessId);

        public static void update()
        {
            if (Properties.Settings.Default.isMuted)
            {
                WakeTimer.Stop();
                WaveIn.StopRecording();
            }
            else
            {
                try
                {
                    WaveIn.StartRecording();
                }
                catch
                {
                    error1();
                    Logger.warn("Leo couldn't start voice recognition. Microphone access is not allowed.");
                    MainWindow.MicAccess = false;
                }
            }
        }

        public static void error1()
        {
            MessageBox.showMessage(Resources.messageBox_errorSign, Resources.system_error1, MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok);
        }

        private static void WaveInOnDataAvailable(object? sender, WaveInEventArgs e)
        {
            _writer?.Write(e.Buffer, 0, e.BytesRecorded);

            var vosk = new VoskRecognizer();

            if (Recognizer!.AcceptWaveform(e.Buffer, e.BytesRecorded))
            {
                var pResult = JObject.Parse(Recognizer.Result());
                RecognizedText = pResult["text"]!.ToString();
                vosk.speechRecognized();
            }
            else
            {
                var pResult = JObject.Parse(Recognizer.PartialResult());
                RecognizedText = pResult["partial"]!.ToString();
                vosk.speechRecognized();
            }
        }
        
        public void speechRecognized()
        {
            if (RecognizedText != string.Empty)
            {
                Console.WriteLine($@"[VOSK] Recognized > {RecognizedText}");
            }

            if (WakeTimer.Elapsed.Seconds >= 15 && _wakeWordStatus && Busy == false)
            {
                Sounds.playMedia(@".\Assets\Sounds\stop.wav");
                _wakeWordStatus = false;

                _dispatcher?.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)Home.deactivateAnimation);
                
                Logger.message("Assistant deactivated");
                
                WakeTimer.Stop();
                WakeTimer.Reset();
            }

            if (_wakeWordStatus)
            {
                foreach (var command in Command.CommandsCollection!)
                {
                    if (RecognizedText!.Contains(command.Phrase!, StringComparison.CurrentCultureIgnoreCase))
                    {
                        switch (command.Type)
                        {
                            case "1":
                                var folder = Environment.ExpandEnvironmentVariables(command.Reference!);

                                Scripts.startProgram(folder,
                                    command.VoiceFile!,
                                    $@".\Assets\Voices\errors\err{command.ErrorNumber}.wav",
                                    command.ReplyMessage!);
                                break;
                            case "2":
                                Scripts.openWebsite(command.Reference!,
                                    command.VoiceFile!,
                                    $@".\Assets\Voices\errors\err{command.ErrorNumber}.wav",
                                    command.ReplyMessage!);
                                break;
                        }
                    }
                }
            }
            
            // WAKE WORD
            if (RecognizedText!.Contains("лео") || RecognizedText == "лео")
            {
                WakeTimer.Reset();
                WakeTimer.Start();

                if (!_wakeWordStatus)
                {
                    Sounds.playMedia(@".\Assets\Sounds\start.wav");
                    _dispatcher?.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                    {
                        if (RecognizedText!.Length > 3)
                        { Chat.addMessageItem("Лео", "Right"); }
                        Home.activateAnimation();
                    });
                }

                _wakeWordStatus = true; 

                Logger.message("Assistant activated");
            }
            
            if (RecognizedText == "спасибо" && !Busy && _wakeWordStatus)
            {
                Busy = true;
                WakeTimer.Restart();

                Sounds.playMedia(@".\Assets\Voices\thanksYou.wav");
                Chat.initialMessage("Всегда пожалуйста!");
                
                Logger.message($"Vosk recognized the phrase - {RecognizedText}");
                
                RecognizedText = "";

                Recognizer?.Reset();
                Busy = false;
            }

            if (RecognizedText == "алиса" && !Busy)
            {
                incorrectWakeWord(@".\Assets\Voices\denial\alica.wav", "Алиса");
            }

            if (RecognizedText == "сири" && !Busy)
            {
                incorrectWakeWord(@".\Assets\Voices\denial\siri.wav", "Siri");
            }

            if (RecognizedText == "маруся" && !Busy)
            {
                incorrectWakeWord(@".\Assets\Voices\denial\marusa.wav", "Маруся");
            }

            // Очистка корзины
            if (RecognizedText.Contains("очисти корзину") && !Busy && _wakeWordStatus)
            {
                Busy = true;
                WakeTimer.Restart();

                if (Properties.Settings.Default.allowComputerControl)
                {
                    var result = SHEmptyRecycleBin(IntPtr.Zero, null, 0);
                    if (result == 0)
                    {
                        Sounds.playMedia(@".\Assets\Voices\bin_messages\bin1.wav");
                        Chat.initialMessage("Корзина очищена");
                    }
                    else
                    {
                        Sounds.playMedia(@".\Assets\Voices\bin_messages\bin2.wav");
                        Chat.initialMessage("Корзина уже пуста!");
                    }
                }
                else
                {
                    Sounds.playMedia(@".\Assets\Voices\err1.wav");
                    Chat.initialMessage("Мне запрещено делать это");
                }
                Recognizer?.Reset();
                Busy = false;

            }
            
            if (RecognizedText.Contains("закрой") && !Busy && _wakeWordStatus)
            {
                Busy = true;
                WakeTimer.Restart();

                if (Properties.Settings.Default.allowComputerControl)
                {
                    Sounds.playMedia(@".\Assets\Voices\good.wav");
                    Chat.initialMessage("Хорошо");

                    IntPtr hWnd = GetForegroundWindow();
                    GetWindowThreadProcessId(hWnd, out var processId);

                    Process proc = Process.GetProcessById(processId);
                    proc.Kill();
                }
                else
                {
                    Sounds.playMedia(@".\Assets\Voices\errors\err1.wav");
                    Chat.initialMessage("Мне запрещено делать это");
                }
                Recognizer?.Reset();
                Busy = false;
            }

            if (RecognizedText.Contains("поставь на паузу") && !Busy && _wakeWordStatus)
            {
                Scripts.musicInteraction(Scripts.MusicInteractionVariations.Pause);
            }

            if (RecognizedText.Contains("включи обратно") && !Busy && _wakeWordStatus)
            {
                Scripts.musicInteraction(Scripts.MusicInteractionVariations.Play);
            }

            if (RecognizedText.Contains("предыдущий трек") && !Busy && _wakeWordStatus)
            {
                Scripts.musicInteraction(Scripts.MusicInteractionVariations.PreviousTrack);
            }

            if (RecognizedText.Contains("следующий трек") && !Busy && _wakeWordStatus)
            {
                Scripts.musicInteraction(Scripts.MusicInteractionVariations.NextTrack);
            }
        }
        

        private void incorrectWakeWord(string voiceFile, string wakeWord)
        {
            Busy = true;
            WakeTimer.Restart();
                
            Sounds.playMedia(voiceFile);
            Chat.initialMessage($"Я не {wakeWord}! Я Лео!");

            Recognizer?.Reset();
            Busy = false;
        }
    }
}