using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RotateScreen;

public class TrayApp : ApplicationContext
{
    private readonly NotifyIcon _tray;
    private readonly FloatingButton _btn;

    public TrayApp()
    {
        AutoStart.Enable();

        _btn = new FloatingButton();
        _btn.ExitRequested = ExitApp;
        _btn.Show();

        _tray = new NotifyIcon
        {
            Icon    = BuildTrayIcon(),
            Text    = "RotateScreen — 左键旋转 / 右键显示隐藏",
            Visible = true,
        };

        _tray.MouseClick += (_, e) =>
        {
            if (e.Button == MouseButtons.Left)
            {
                Display.Toggle();
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (_btn.Visible) _btn.Hide();
                else _btn.Show();
            }
        };
    }

    private static Icon BuildTrayIcon()
    {
        using var bmp = new Bitmap(32, 32);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            // Solid black disc
            using (var bg = new SolidBrush(Color.Black))
                g.FillEllipse(bg, 0, 0, 32, 32);

            // White rotate glyph
            GlyphRenderer.Draw(g, new RectangleF(4, 4, 24, 24), Color.White, 2.2f);
        }

        IntPtr h = bmp.GetHicon();
        return Icon.FromHandle(h);
    }

    private void ExitApp()
    {
        _tray.Visible = false;
        _tray.Dispose();
        _btn.Dispose();
        ExitThread();
    }
}
