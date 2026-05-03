using System;
using System.Runtime.InteropServices;

namespace RotateScreen;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct DEVMODE
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string dmDeviceName;
    public short dmSpecVersion;
    public short dmDriverVersion;
    public short dmSize;
    public short dmDriverExtra;
    public int dmFields;
    public int dmPositionX;
    public int dmPositionY;
    public int dmDisplayOrientation;
    public int dmDisplayFixedOutput;
    public short dmColor;
    public short dmDuplex;
    public short dmYResolution;
    public short dmTTOption;
    public short dmCollate;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string dmFormName;
    public short dmLogPixels;
    public int dmBitsPerPel;
    public int dmPelsWidth;
    public int dmPelsHeight;
    public int dmDisplayFlags;
    public int dmDisplayFrequency;
    public int dmICMMethod;
    public int dmICMIntent;
    public int dmMediaType;
    public int dmDitherType;
    public int dmReserved1;
    public int dmReserved2;
    public int dmPanningWidth;
    public int dmPanningHeight;
}

internal static class Display
{
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int EnumDisplaySettings(string? deviceName, int modeNum, ref DEVMODE devMode);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int ChangeDisplaySettingsEx(string? deviceName, ref DEVMODE devMode, IntPtr hwnd, int dwflags, IntPtr lParam);

    public const int ENUM_CURRENT_SETTINGS = -1;
    public const int CDS_UPDATEREGISTRY    = 0x01;
    public const int DMDO_DEFAULT = 0;
    public const int DMDO_90      = 1;
    public const int DMDO_180     = 2;
    public const int DMDO_270     = 3;

    public static int GetOrientation()
    {
        var dm = new DEVMODE();
        dm.dmSize = (short)Marshal.SizeOf(dm);
        if (EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref dm) == 0) return DMDO_DEFAULT;
        return dm.dmDisplayOrientation;
    }

    public static void RotateTo(int target)
    {
        var dm = new DEVMODE();
        dm.dmSize = (short)Marshal.SizeOf(dm);
        if (EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref dm) == 0) return;
        if (dm.dmDisplayOrientation == target) return;

        bool curPortrait    = dm.dmDisplayOrientation == DMDO_90 || dm.dmDisplayOrientation == DMDO_270;
        bool targetPortrait = target == DMDO_90 || target == DMDO_270;
        if (curPortrait != targetPortrait)
            (dm.dmPelsWidth, dm.dmPelsHeight) = (dm.dmPelsHeight, dm.dmPelsWidth);

        dm.dmDisplayOrientation = target;
        ChangeDisplaySettingsEx(null, ref dm, IntPtr.Zero, CDS_UPDATEREGISTRY, IntPtr.Zero);
    }

    public static void Toggle()
    {
        int cur = GetOrientation();
        int next = (cur == DMDO_DEFAULT || cur == DMDO_180) ? DMDO_90 : DMDO_DEFAULT;
        RotateTo(next);
    }
}
