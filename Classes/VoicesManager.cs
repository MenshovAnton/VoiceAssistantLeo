using System.IO;

namespace Leo.Classes;

public class VoicesManager
{
    private const string VoicesDirection = @".\Assets\Voices\";
    private string[] _voiceFiles = Directory.GetFiles(VoicesDirection, "*.*", SearchOption.AllDirectories);

    public string[] getVoiceFilesList()
    {
        return _voiceFiles;
    }

    public int getIndexOfVoiceFile(string fileName)
    {
        return Array.IndexOf(_voiceFiles, fileName);
    }

    public void updateVoicesFilesList()
    {
        _voiceFiles = Directory.GetFiles(VoicesDirection, "*.*", SearchOption.AllDirectories);
    }
}