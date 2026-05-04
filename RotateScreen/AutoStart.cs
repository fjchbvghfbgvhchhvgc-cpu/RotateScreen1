using System;
using Microsoft.Win32;

namespace RotateScreen;

internal static class AutoStart
{
    private const string RunKey  = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "RotateScreen";

    public static void Enable()
    {
        try
        {
            string? exe = Environment.ProcessPath;
            if (string.IsNullOrEmpty(exe)) return;

            using var key = Registry.CurrentUser.OpenSubKey(RunKey, writable: true);
            if (key == null) return;

            string desired = $"\"{exe}\"";
            string? current = key.GetValue(AppName) as string;
            if (!string.Equals(current, desired, StringComparison.OrdinalIgnoreCase))
                key.SetValue(AppName, desired, RegistryValueKind.String);
        }
        catch
        {
            // best-effort; ignore failures (e.g. group policy blocking the Run key)
        }
    }
}
