using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Threading;
using Windows.Media.Control;
using Leo.PageModels;
using Leo.Properties;
using Leo.WindowModels;
using NAudio.Wave;
using Newtonsoft.Json.Linq;
using Vosk;
using MessageBox = Leo.WindowModels.MessageBox;
using Settings = Leo.PageModels.Settings;

namespace Leo.Classes
{
    public class CommandData
    {
        public string? Phrase { get; init; }
        public string? Reference { get; init; }
        public string? Type { get; init; }
        public string? VoiceFile { get; init; }
        public string? ErrorNumber { get; init; }
        public string? ReplyMessage { get; init; }
    }
    
    public class Vosk
    {
        public static ObservableCollection<CommandData>? Commands { get; set; }
        
        private static Dispatcher? _dispatcher;

        private static VoskRecognizer? _recognizer; // Объект распознавания VOSK
        private static WaveFileWriter? _writer; // Объект записи с микрофона
        private static bool _busy;

        public static string? RecognizedText;
        private static bool _wakeWordStatus;
        
        private static readonly Logger Logger = new();
        private static readonly Stopwatch WakeTimer = new();
        private static readonly WaveInEvent WaveIn = new();
        
        private enum RecycleFlags : uint;
        
        private readonly MediaPlayer _player = new();
        private static readonly MessageBox MessageBox = new();

        public static void main()
        {
            // Инициализация модели
            var model = new Model(".\\VoskModel");
            _recognizer = new VoskRecognizer(model, 16000f);

            // Инициализация записи
            WaveIn.WaveFormat = new WaveFormat(16000, 1);
            WaveIn.DataAvailable += WaveInOnDataAvailable;

            // Временный файл записи голоса
            _writer = new WaveFileWriter(".\\voice.wav", WaveIn.WaveFormat);

            var currentDispatcher = Dispatcher.CurrentDispatcher;
            _dispatcher = currentDispatcher;
            
            Commands = [];
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
        
        public static void addCommand(string type, string methodRef, string? phrase, string? voiceFile, string? errorNumber, string? replyMessage)
        {
            Commands!.Add(new CommandData()
            {
                Phrase = phrase,
                Reference = methodRef,
                Type = type,
                VoiceFile = voiceFile,
                ErrorNumber = errorNumber,
                ReplyMessage = replyMessage
            });
        }

        public static void error1()
        {
            MessageBox.showMessage(Resources.messageBox_errorSign, Resources.system_error1, MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok);
        }

        private static void WaveInOnDataAvailable(object? sender, WaveInEventArgs e)
        {
            _writer?.Write(e.Buffer, 0, e.BytesRecorded);

            var vosk = new Vosk();

            if (_recognizer!.AcceptWaveform(e.Buffer, e.BytesRecorded))
            {
                // Парсинг объекта с текстом
                var pResult = JObject.Parse(_recognizer.Result());
                RecognizedText = pResult["text"]!.ToString();
                vosk.speechRecognized(); // Проверка результатов
            }
            else
            {
                // Парсинг объекта с текстом
                var pResult = JObject.Parse(_recognizer.PartialResult());
                RecognizedText = pResult["partial"]!.ToString();
                vosk.speechRecognized(); // Проверка результатов
            }
        }
        
        public void speechRecognized()
        {
            if (RecognizedText != string.Empty)
            {
                Console.WriteLine($@"[VOSK] Recognized > {RecognizedText}");
            }

            if (WakeTimer.Elapsed.Seconds >= 15 && _wakeWordStatus)
            {
                playSound(@".\Assets\Sounds\stop.wav");
                _wakeWordStatus = false;

                _dispatcher?.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)Home.deactivateAnimation);
                
                Logger.message("Assistant deactivated");
                
                WakeTimer.Stop();
                WakeTimer.Reset();
            }

            // WAKE WORD
            if (RecognizedText!.Contains("лео") || RecognizedText == "лео")
            {
                WakeTimer.Reset();
                WakeTimer.Start();

                if (!_wakeWordStatus)
                {
                    playSound(@".\Assets\Sounds\start.wav");
                    _dispatcher?.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
                    {
                        if (RecognizedText!.Length > 3)
                        { Chat.addMessage("Лео", "Right"); }
                        Home.activateAnimation();
                    });
                }

                _wakeWordStatus = true; 

                Logger.message("Assistant activated");
            }

            if (_wakeWordStatus)
            {
                foreach (var command in Commands!)
                {
                    if (RecognizedText.Contains(command.Phrase!.ToLower()))
                    {
                        switch (command.Type)
                        {
                            case "1":
                                string folder = Environment.ExpandEnvironmentVariables(command.Reference!);

                                startProgram(folder,
                                    command.VoiceFile!,
                                    $@".\Assets\Voices\errors\err{command.ErrorNumber}.wav",
                                    4,
                                    command.ReplyMessage!);
                                break;
                            case "2":
                                openWebsite(command.Reference!,
                                    command.VoiceFile!,
                                    $@".\Assets\Voices\errors\err{command.ErrorNumber}.wav",
                                    command.ReplyMessage!);
                                break;
                        }
                    }
                }
            }
            
            if (RecognizedText == "спасибо" && !_busy && _wakeWordStatus)
            {
                _busy = true;
                WakeTimer.Restart();

                playVoice(@".\Assets\Voices\thanksYou.wav");
                initialMessage("Всегда пожалуйста!", "Left");
                
                Logger.message($"Vosk recognized the phrase - {RecognizedText}");
                
                RecognizedText = "";

                _recognizer?.Reset();
                _busy = false;
            }

            if (RecognizedText == "алиса" && !_busy)
            {
                incorrectWakeWord(@".\Assets\Voices\denial\alica.wav", "Алиса");
            }

            if (RecognizedText == "сири" && !_busy)
            {
                incorrectWakeWord(@".\Assets\Voices\denial\siri.wav", "Siri");
            }

            if (RecognizedText == "маруся" && !_busy)
            {
                incorrectWakeWord(@".\Assets\Voices\denial\marusa.wav", "Маруся");
            }

            // Очистка корзины
            if (RecognizedText.Contains("очисти корзину") && !_busy && _wakeWordStatus)
            {
                _busy = true;
                WakeTimer.Restart();

                if (Properties.Settings.Default.allowComputerControl)
                {
                    var result = SHEmptyRecycleBin(IntPtr.Zero, null, 0);
                    if (result == 0)
                    {
                        playVoice(@".\Assets\Voices\bin_messages\bin1.wav");
                        initialMessage("Корзина очищена", "Left");
                    }
                    else
                    {
                        playVoice(@".\Assets\Voices\bin_messages\bin2.wav");
                        initialMessage("Корзина уже пуста!", "Left");
                    }
                }
                else
                {
                    playVoice(@".\Assets\Voices\err1.wav");
                    initialMessage("Мне запрещено делать это", "Left");
                }
                _recognizer?.Reset();
                _busy = false;

            }
            
            if (RecognizedText.Contains("закрой") && !_busy && _wakeWordStatus)
            {
                _busy = true;
                WakeTimer.Restart();

                if (Properties.Settings.Default.allowComputerControl)
                {
                    playVoice(@".\Assets\Voices\good.wav");
                    initialMessage("Хорошо", "Left");

                    IntPtr hWnd = GetForegroundWindow();
                    GetWindowThreadProcessId(hWnd, out var processId);

                    Process proc = Process.GetProcessById(processId);
                    proc.Kill();
                }
                else
                {
                    playVoice(@".\Assets\Voices\errors\err1.wav");
                    initialMessage("Мне запрещено делать это", "Left");
                }
                _recognizer?.Reset();
                _busy = false;
            }

            if (RecognizedText.Contains("поставь на паузу") && !_busy && _wakeWordStatus)
            {
                musicInteraction(InteractionVariations.Pause);
            }

            if (RecognizedText.Contains("включи обратно") && !_busy && _wakeWordStatus)
            {
                musicInteraction(InteractionVariations.Play);
            }

            if (RecognizedText.Contains("предыдущий трек") && !_busy && _wakeWordStatus)
            {
                musicInteraction(InteractionVariations.PreviousTrack);
            }

            if (RecognizedText.Contains("следующий трек") && !_busy && _wakeWordStatus)
            {
                musicInteraction(InteractionVariations.NextTrack);
            }
        }

        private enum InteractionVariations
        {
            Play,
            Pause,
            PreviousTrack,
            NextTrack
        }
        
        private async void musicInteraction(Enum interactionVariations)
        {
            _busy = true;
            WakeTimer.Restart();

            if (Properties.Settings.Default.allowComputerControl)
            {
                playVoice(@".\Assets\Voices\good.wav");
                
                var mediaTransportManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                var mediaSession = mediaTransportManager.GetCurrentSession();

                try
                {
                    switch (interactionVariations.ToString())
                    {
                        case "Play":
                            await mediaSession.TryPlayAsync();
                            break;
                        case "Pause":
                            await mediaSession.TryPauseAsync();
                            break;
                        case "PreviousTrack":
                            await mediaSession.TrySkipPreviousAsync();
                            break;
                        case "NextTrack":
                            await mediaSession.TrySkipNextAsync();
                            break;
                    }
                }
                catch
                {
                    System.Media.SystemSounds.Exclamation.Play();
                }
                
                initialMessage("Хорошо", "Left");
            }
            else
            {
                playVoice(@".\Assets\Voices\errors\err1.wav");
                initialMessage("Мне запрещено делать это", "Left");
            }
                
            _recognizer?.Reset();
            _busy = false;
        }
        

        private void incorrectWakeWord(string voiceFile, string wakeWord)
        {
            _busy = true;
            WakeTimer.Restart();
                
            playVoice(voiceFile);
            initialMessage($"Я не {wakeWord}! Я Лео!", "Left");

            _recognizer?.Reset();
            _busy = false;
        }
        
        
        private void startProgram(string target, string media, string error, int rndInt, string mesText)
        {
            _busy = true;
            WakeTimer.Restart();

            if (Properties.Settings.Default.allowProgrammsStart)
            {
                try
                {
                    playVoice(media);
                    initialMessage(mesText, "Left");

                    var p = new Process();
                    p.StartInfo.FileName = target;
                    p.Start();
                    RecognizedText = "";

                }
                catch (System.ComponentModel.Win32Exception)
                {
                    playVoice(error);
                    initialMessage("Приложение не найдено на вашем устройстве!", "Left");
                    
                    Logger.error("Leo was unable to open the program. The program was not found on the device.");
                    
                    RecognizedText = "";
                }
            }
            else
            {
                playVoice(@".\voices\err1.wav");
                initialMessage("Мне запрещено делать это", "Left");
            }

            _recognizer?.Reset();
            _busy = false;
        }

        private void openWebsite(string url, string media, string error, string mesText)
        {
            _busy = true;
            WakeTimer.Restart();

            if (Properties.Settings.Default.allowBrowserStart)
            {
                playVoice(media);
                initialMessage(mesText, "Left");

                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
                RecognizedText = "";

            }
            else
            {
                playVoice(error);
                initialMessage("Мне запрещено делать это", "Left");
            }

            _recognizer?.Reset();
            _busy = false;
        }

        private void playSound(string file)
        {
            _player.Open(new Uri(file, UriKind.Relative));
            _player.Volume = Settings.SoundVolume / 100.0f;
            _player.Play();
        }

        private void playVoice(string file)
        {
            _player.Open(new Uri(file, UriKind.Relative));
            _player.Volume = Settings.VoiceVolume / 100.0f;
            _player.Play();
        }

        private static void initialMessage(string message, string alignment)
        {
            var recognizedText = RecognizedText![..1].ToUpper() + (RecognizedText.Length > 1 ? RecognizedText[1..] : "");
            
            _dispatcher?.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                Chat.addMessage(recognizedText, "Right");
                Chat.addMessage(message, alignment);
            });
        }
        
    }
}