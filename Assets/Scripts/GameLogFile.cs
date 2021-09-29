using System.IO;

public static class GameLogFile
{
    static string directory = Directory.GetCurrentDirectory();
    static string logName = "gameLog";
    static public void WriteToLog(ulong clientID, string input)
    {
        var path = GetPath(clientID);

        if (!File.Exists(path))
        {
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.WriteLine(input);
            }

        }
        else
        {
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(input);
            }
        }
    }

    static string GetPath(ulong clientID)
    {
        return Path.Combine(directory, $"{logName}_{clientID}.txt");
    }
}
