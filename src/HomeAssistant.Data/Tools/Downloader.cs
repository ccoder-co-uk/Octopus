namespace HomeAssistant.Data.Tools;

internal class Downloader
{
    public static void Download(string from, string to)
    {
        ClearDownOldFiles(to);
        FetchNewFiles(from, to);
    }

    static void ClearDownOldFiles(string inFolder)
    {
        Console.WriteLine($"Clearing the working directory ...");

        foreach (string oldFile in Directory.GetDirectories(inFolder))
            Directory.Delete(oldFile, true);

        foreach (string oldFile in Directory.GetFiles(inFolder))
            File.Delete(oldFile);
    }

    static void FetchNewFiles(string from, string to)
    {
        Console.WriteLine($"Downloading files from {from} ...");

        if (!Directory.Exists(to))
            Directory.CreateDirectory(to);

        foreach (var filePath in Directory.GetFiles(from))
            File.Copy(filePath, Path.Combine(to, filePath.Split('\\').Last()), true);

        Console.WriteLine($"Download complete!");
    }
}