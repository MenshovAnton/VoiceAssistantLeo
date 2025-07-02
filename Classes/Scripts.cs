using System.Diagnostics;
using System.IO;
using Windows.Media.Control;
using Leo.PageModels;
using Leo.Properties;
using Leo.WindowModels;

namespace Leo.Classes;

public abstract class Scripts
{
    private static readonly Logger Logger = new();
    private static readonly MessageBox MessageBox = new();
    
    public static void openWebsite(string url, string media, string error, string mesText)
    {
        VoskRecognizer.Busy = true;
        VoskRecognizer.WakeTimer.Restart();

        if (Properties.Settings.Default.allowBrowserStart)
        {
            Sounds.playMedia(media);
            Chat.initialMessage(mesText);

            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
            VoskRecognizer.RecognizedText = "";

        }
        else
        {
            Sounds.playMedia(error);
            Chat.initialMessage("Мне запрещено делать это");
        }

        VoskRecognizer.Recognizer?.Reset();
        VoskRecognizer.Busy = false;
    }
    
    public static void startProgram(string target, string media, string error, string mesText)
    {
        VoskRecognizer.Busy = true;
        VoskRecognizer.WakeTimer.Restart();

        if (Properties.Settings.Default.allowProgrammsStart)
        {
            try
            {
                Sounds.playMedia(media);
                Chat.initialMessage(mesText);

                var p = new Process();
                p.StartInfo.FileName = target;
                p.Start();
                VoskRecognizer.RecognizedText = "";

            }
            catch (System.ComponentModel.Win32Exception)
            {
                Sounds.playMedia(error);
                Chat.initialMessage("Приложение не найдено на вашем устройстве!");
                    
                Logger.error("Leo was unable to open the program. The program was not found on the device.");
                    
                VoskRecognizer.RecognizedText = "";
            }
        }
        else
        {
            Sounds.playMedia(@".\voices\err1.wav");
            Chat.initialMessage("Мне запрещено делать это");
        }

        VoskRecognizer.Recognizer?.Reset();
        VoskRecognizer.Busy = false;
    }
    
    public static async void musicInteraction(Enum interactionVariations)
    {
        try
        {
            VoskRecognizer.Busy = true;
            VoskRecognizer.WakeTimer.Restart();

            if (Properties.Settings.Default.allowComputerControl)
            {
                Sounds.playMedia(@".\Assets\Voices\good.wav");
                
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
                
                Chat.initialMessage("Хорошо");
            }
            else
            {
                Sounds.playMedia(@".\Assets\Voices\errors\err1.wav");
                Chat.initialMessage("Мне запрещено делать это");
            }
                
            VoskRecognizer.Recognizer?.Reset();
            VoskRecognizer.Busy = false;
        }
        catch (Exception ex)
        {
            Logger.error("Async error in music interaction\n" + ex);
            MessageBox.showMessage(Resources.messageBox_errorSign, Resources.system_message5,
                MessageBox.MessageBoxType.Error, MessageBox.MessageBoxButtons.Ok);
        }
    }

    public enum MusicInteractionVariations
    {
        Play,
        Pause,
        PreviousTrack,
        NextTrack
    }
    
    public static bool isValidPathOrUrl(string input)
    {
        if (Uri.IsWellFormedUriString(input, UriKind.Absolute))
            return true;
        
        if (File.Exists(Environment.ExpandEnvironmentVariables(input)))
            return true;
        
        if (input == "cmd.exe")
            return true;

        return false;
    }
}