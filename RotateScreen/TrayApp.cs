using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace RotateScreen;

public class TrayApp : ApplicationContext
{
    private readonly NotifyIcon _tray;
    private readonly FloatingButton _btn;
    private readonly ToolStripMenuItem _showItem;

    public TrayApp()
    {
        _btn = new FloatingButton();
        _btn.Show();

        _showItem = new ToolStripMenuItem("显示悬浮按钮 / Show floating button")
        {
            Checked = true,
            CheckOnClick = true
        };
        _showItem.CheckedChanged += (_, _) =>
        {
            if (_showItem.Checked) _btn.Show();
            else _btn.Hide();
        };

        var menu = new ContextMenuStrip();
        menu.Items.Add("旋转到 0° (横屏 / Landscape)",      null, (_, _) => Display.RotateTo(Display.DMDO_DEFAULT));
        menu.Items.Add("旋转到 90° (竖屏 / Portrait)",       null, (_, _) => Display.RotateTo(Display.DMDO_90));
        menu.Items.Add("旋转到 180° (倒置 / Landscape flip)", null, (_, _) => Display.RotateTo(Display.DMDO_180));
        menu.Items.Add("旋转到 270° (竖屏反向 / Portrait flip)", null, (_, _) => Display.RotateTo(Display.DMDO_270));
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(_showItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("退出 / Exit", null, (_, _) => ExitApp());

        _tray = new NotifyIcon
        {
            Icon = BuildIcon(),
            Text = "RotateScreen — 横竖屏切换",
            Visible = true,
            ContextMenuStrip = menu
        };

        _tray.MouseClick += (_, e) =>
        {
            if (e.Button == MouseButtons.Left) Display.Toggle();
        };
    }

    private static Icon BuildIcon()
    {
        using var bmp = new Bitmap(32, 32);
        using (var g = Graphics.FromImage(bmp))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            g.Clear(Color.Transparent);

            using var font = new Font("Segoe UI Symbol", 26f, FontStyle.Bold, GraphicsUnit.Pixel);
            const string glyph = "⟳";
            var sz = g.MeasureString(glyph, font);
            using var brush = new SolidBrush(Color.White);
            g.DrawString(glyph, font, brush,
                (32 - sz.Width)  / 2f,
                (32 - sz.Height) / 2f - 1);
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
