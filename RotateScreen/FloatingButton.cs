using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RotateScreen;

public class FloatingButton : Form
{
    private const double IdleOpacity  = 0.55;
    private const double HoverOpacity = 0.90;

    private Point _dragStart;
    private bool _dragging;
    private bool _moved;

    public Action? ExitRequested;

    public FloatingButton()
    {
        FormBorderStyle = FormBorderStyle.None;
        TopMost = true;
        ShowInTaskbar = false;
        BackColor = Color.Black;
        Size = new Size(46, 46);
        Opacity = IdleOpacity;
        DoubleBuffered = true;
        StartPosition = FormStartPosition.Manual;

        var wa = Screen.PrimaryScreen!.WorkingArea;
        Location = new Point(wa.Right - Width - 24, wa.Bottom - Height - 24);

        using var path = new GraphicsPath();
        path.AddEllipse(0, 0, Width, Height);
        Region = new Region(path);

        var menu = new ContextMenuStrip();
        menu.Items.Add("退出 / Exit", null, (_, _) => ExitRequested?.Invoke());
        ContextMenuStrip = menu;

        MouseEnter += (_, _) => Opacity = HoverOpacity;
        MouseLeave += (_, _) => { if (!_dragging) Opacity = IdleOpacity; };
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;

        // Dark translucent disc background
        using (var bg = new SolidBrush(Color.FromArgb(36, 36, 36)))
            g.FillEllipse(bg, 0, 0, Width, Height);

        // Rotate glyph (ring + dot + arrow) in white
        var inner = new RectangleF(7, 7, Width - 14, Height - 14);
        GlyphRenderer.Draw(g, inner, Color.White, 2.2f);
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _dragStart = e.Location;
            _dragging  = true;
            _moved     = false;
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
            Opacity = IdleOpacity;
        }
        base.OnMouseUp(e);
    }
}
