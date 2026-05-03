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
    public const int CDS_UPDATEREGISTRY  = 0x01;
    public const int DMDO_DEFAULT = 0;
    public const int DMDO_90      = 1;
    public const int DMDO_180     = 2;
    public const int DMDO_270     = 3;
}

internal static class Program
{
    [STAThread]
    private static int Main()
    {
        var dm = new DEVMODE();
        dm.dmSize = (short)Marshal.SizeOf(dm);

        if (Display.EnumDisplaySettings(null, Display.ENUM_CURRENT_SETTINGS, ref dm) == 0)
        {
            return 1;
        }

        int newOrient = (dm.dmDisplayOrientation == Display.DMDO_DEFAULT
                      || dm.dmDisplayOrientation == Display.DMDO_180)
                      ? Display.DMDO_90
                      : Display.DMDO_DEFAULT;

        (dm.dmPelsWidth, dm.dmPelsHeight) = (dm.dmPelsHeight, dm.dmPelsWidth);
        dm.dmDisplayOrientation = newOrient;

        return Display.ChangeDisplaySettingsEx(
            null, ref dm, IntPtr.Zero,
            Display.CDS_UPDATEREGISTRY, IntPtr.Zero);
    }
}
