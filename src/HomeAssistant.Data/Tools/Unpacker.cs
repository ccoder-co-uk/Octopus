using System.Diagnostics;

namespace HomeAssistant.Data.Tools;

internal static class Unpacker
{
    public static async Task<string> Unpack(string sourceTar)
    {
        var fileName = sourceTar.Split("\\")[^1];
        var workingDir = sourceTar.TrimEnd(fileName.ToCharArray());
        var destination = sourceTar.Replace(fileName, fileName.Trim([.. ".tar"]));
        Directory.CreateDirectory(destination);

        await Process.Start("cmd.exe", $"/C chdir \"{workingDir}\"&tar -xf \"{sourceTar}\" -C \"{destination}\"")
            .WaitForExitAsync();

        var gzFilePath = Directory.EnumerateFiles(destination, "homeassistant.tar.gz", SearchOption.AllDirectories)
            .FirstOrDefault();

        if (gzFilePath is not null)
        {
            fileName = gzFilePath.Split("\\")[^1];
            destination = gzFilePath.Replace(fileName, fileName.Trim([.. ".tar.gz"]));
            Directory.CreateDirectory(destination);

            await Process.Start("cmd.exe", $"/C chdir \"{workingDir}\"&tar -xf \"{gzFilePath}\" -C \"{destination}\"")
                .WaitForExitAsync();

            return Directory.EnumerateFiles(destination, "home-assistant_v2.db", SearchOption.AllDirectories)
                .FirstOrDefault();
        }

        return null;
    }
}