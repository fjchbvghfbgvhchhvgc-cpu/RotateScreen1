using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace RotateScreen;

internal static class GlyphRenderer
{
    public static void Draw(Graphics g, RectangleF bounds, Color color, float lineWidth)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;

        float cx = bounds.X + bounds.Width  / 2f;
        float cy = bounds.Y + bounds.Height / 2f;
        float r  = Math.Min(bounds.Width, bounds.Height) / 2f - lineWidth - 1f;

        using var pen   = new Pen(color, lineWidth) { StartCap = LineCap.Round, EndCap = LineCap.Flat };
        using var brush = new SolidBrush(color);

        // Ring with a 70° gap at the top-right (sweeps 290° clockwise from 3 o'clock)
        g.DrawArc(pen, cx - r, cy - r, r * 2, r * 2, 0f, 290f);

        // Arrow tip at the end of the arc, pointing clockwise (tangentially)
        float endRad = 290f * (float)(Math.PI / 180);
        float ex = cx + r * (float)Math.Cos(endRad);
        float ey = cy + r * (float)Math.Sin(endRad);

        float tx = -(float)Math.Sin(endRad);   // tangent (clockwise direction)
        float ty =  (float)Math.Cos(endRad);
        float ox =  (float)Math.Cos(endRad);   // radial outward
        float oy =  (float)Math.Sin(endRad);

        float aw = lineWidth * 2.4f;
        var tip  = new PointF(ex + tx * aw,        ey + ty * aw);
        var outP = new PointF(ex + ox * aw * 0.7f, ey + oy * aw * 0.7f);
        var inP  = new PointF(ex - ox * aw * 0.7f, ey - oy * aw * 0.7f);
        g.FillPolygon(brush, new[] { tip, outP, inP });

        // Center dot
        float dotR = lineWidth * 0.95f;
        g.FillEllipse(brush, cx - dotR, cy - dotR, dotR * 2, dotR * 2);
    }
}
