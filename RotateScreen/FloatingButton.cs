using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RotateScreen;

public class FloatingButton : Form
{
    private Point _dragStart;
    private bool _dragging;
    private bool _moved;

    public FloatingButton()
    {
        FormBorderStyle = FormBorderStyle.None;
        TopMost = true;
        ShowInTaskbar = false;
        BackColor = Color.FromArgb(0, 102, 204);
        Size = new Size(64, 64);
        DoubleBuffered = true;
        StartPosition = FormStartPosition.Manual;

        var wa = Screen.PrimaryScreen!.WorkingArea;
        Location = new Point(wa.Right - Width - 24, wa.Bottom - Height - 24);

        using var path = new GraphicsPath();
        path.AddEllipse(0, 0, Width, Height);
        Region = new Region(path);

        var menu = new ContextMenuStrip();
        menu.Items.Add("旋转屏幕 / Rotate", null, (_, _) => Display.Toggle());
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("退出 / Exit", null, (_, _) => Application.Exit());
        ContextMenuStrip = menu;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

        var rect = new Rectangle(0, 0, Width, Height);
        using (var body = new LinearGradientBrush(rect,
                   Color.FromArgb(0, 153, 255),
                   Color.FromArgb(0, 92, 184),
                   90f))
        {
            g.FillEllipse(body, rect);
        }

        using (var ring = new Pen(Color.FromArgb(220, 255, 255, 255), 2f))
        {
            g.DrawEllipse(ring, 1, 1, Width - 2, Height - 2);
        }

        using var font = new Font("Segoe UI Symbol", 30f, FontStyle.Bold, GraphicsUnit.Pixel);
        var glyph = "⟳";
        var sz = g.MeasureString(glyph, font);
        using var text = new SolidBrush(Color.White);
        g.DrawString(glyph, font, text,
            (Width - sz.Width) / 2f,
            (Height - sz.Height) / 2f - 1);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _dragStart = e.Location;
            _dragging = true;
            _moved = false;
        }
        base.OnMouseDown(e);
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        if (_dragging && e.Button == MouseButtons.Left)
        {
            int dx = e.X - _dragStart.X;
            int dy = e.Y - _dragStart.Y;
            if (Math.Abs(dx) > 3 || Math.Abs(dy) > 3) _moved = true;
            if (_moved)
                Location = new Point(Location.X + dx, Location.Y + dy);
        }
        base.OnMouseMove(e);
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left && _dragging)
        {
            _dragging = false;
            if (!_moved) Display.Toggle();
        }
        base.OnMouseUp(e);
    }
}
