
using Microsoft.Win32;

namespace VulnClicker.Models;

public static class StartUpInfoManager
{
    private const string RegistryPath = @"Software\VulnClicker";

    public static StartUpInfo UpdateStartUpInformation()
    {
        DateTime today = DateTime.Now.Date;

        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(RegistryPath);

        DateTime? previousStartUpDate = null;
        int startUpCount = 0;

        if (key != null)
        {
            string? dateText = key.GetValue("StartUpDate") as string;

            if (DateTime.TryParse(dateText, out DateTime parsedDate))
            {
                previousStartUpDate = parsedDate.Date;
            }

            startUpCount = Convert.ToInt32(key.GetValue("StartUpCount", 0));
        }

        startUpCount++;

        using (RegistryKey writeKey =
            Registry.CurrentUser.CreateSubKey(RegistryPath))
        {
            writeKey.SetValue(
                "StartUpDate",
                today.ToString("yyyy/MM/dd"));

            writeKey.SetValue(
                "StartUpCount",
                startUpCount);
        }

        return new StartUpInfo
        {
            Today = today,
            StartUpDate = previousStartUpDate ?? today,
            StartUpCount = startUpCount,
            IsFirstStartUp = previousStartUpDate == null
        };
    }
}