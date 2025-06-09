using System.Windows.Media;
using System.Windows.Threading;

namespace Leo.Classes;

public abstract class Sounds
{
    private static readonly MediaPlayer Player = new();
    private static readonly Logger Logger = new();
    private static readonly Dispatcher? Dispatcher = Dispatcher.CurrentDispatcher;

    public static void updateVolume()
    {
        try
        {
            Dispatcher?.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                Player.Volume = Properties.Settings.Default.voiceVol / 100f;
            });
        }
        catch (Exception ex)
        {
            Logger.error("Failed to change volume level:\n" + ex.Message);
        }
    }

    public static void playMedia(string source)
    {
        try
        {
            Dispatcher?.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate
            {
                Player.Open(new Uri(source, UriKind.Relative));
                Player.Play();
            });
        }
        catch (Exception ex)
        {
            Logger.error("Failed to play media:\n" + ex.Message);
        }
        
    }
}