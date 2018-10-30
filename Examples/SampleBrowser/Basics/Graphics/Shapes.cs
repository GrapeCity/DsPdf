using System;
using System.IO;
using System.Drawing;
using System.Numerics;
using System.Linq;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;

namespace GcPdfWeb.Samples
{
    // Demonstrates how various shapes can be drawn in GcPdf.
    // Shows how simple shapes can be combined to produce more complex shapes.
    // Simple graphics transformations are used to draw some shapes.
    public class Shapes
    {
        // Helper method to draw a polygon and a caption beneath it.
        // Can also be used to just calculate the points without actual drawing.
        // startAngle is for the first point, clockwise from (1,0).
        private PointF[] DrawPolygon(GcGraphics g, PointF center, float r, int n, float startAngle, Pen pen, string caption = null)
        {
            PointF[] pts = new PointF[n];
            for (int i = 0; i < n; ++i)
                pts[i] = new PointF(center.X + (float)(r * Math.Cos(startAngle + 2 * Math.PI * i / n)), center.Y + (float)(r * Math.Sin(startAngle + 2 * Math.PI * i / n)));
            if (pen != null)
                g.DrawPolygon(pts, pen);
            if (!string.IsNullOrEmpty(caption))
                DrawCaption(g, center, r, caption);
            return pts;
        }
        // Helper method to draw a caption beneath a shape:
        private void DrawCaption(GcGraphics g, PointF center, float r, string caption)
        {
            g.DrawString(caption,
                new TextFormat() { Font = StandardFonts.Times, FontSize = 10, },
                new RectangleF(center.X - r, center.Y + r, r * 2, 24),
                TextAlignment.Center, ParagraphAlignment.Center, false);
        }
        // Main entry point.
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.Pages.Add();
            var g = page.Graphics;
            // Document header:
            g.DrawString("Shapes",
                new TextFormat() { Font = StandardFonts.TimesBold, FontSize = 14, Underline = true, },
                new RectangleF(PointF.Empty, new SizeF(page.Size.Width, 44)),
                TextAlignment.Center, ParagraphAlignment.Far);
            // Pen used to draw shapes:
            var pen = new Pen(Color.Orange, 1);
            pen.LineJoin = PenLineJoin.Round;
            int fill = 100; // Surfaces fill alpha

            // Set up the helper layout grid:
            var grid = new
            {
                Cols = 3,
                Rows = 5,
                MarginX = 72,
                MarginY = 36,
                Radius = 36,
                StepX = (page.Size.Width - 144) / 3,
                StepY = (page.Size.Height - 72) / 5,
            };

            // Insertion point of the next figure's center:
            PointF startIp = new PointF(grid.MarginX + grid.StepX / 2, grid.MarginY + grid.StepY / 2 + 10);
            PointF ip = startIp;
            // Debug code to show the layout grid:
            /* 
            var ipp = ip;
            for (int i = 0; i < grid.Cols; ++i)
            {
                ipp.Y = ip.Y;
                for (int j = 0; j < grid.Rows; ++j)
                {
                    g.DrawRectangle(new RectangleF(ipp.X - grid.Radius, ipp.Y - grid.Radius, grid.Radius * 2, grid.Radius * 2), Color.LightGreen, 0.5f);
                    ipp.Y += grid.StepY;
                }
                ipp.X += grid.StepX;
            }
            */

            // Circle:
            g.DrawEllipse(new RectangleF(ip.X - grid.Radius, ip.Y - grid.Radius, grid.Radius * 2, grid.Radius * 2), pen);
            DrawCaption(g, ip, grid.Radius, "Circle");
            ip.X += grid.StepX;

            // Ellipse:
            g.DrawEllipse(new RectangleF(ip.X - grid.Radius * 1.4f, ip.Y - grid.Radius / 2, grid.Radius * 2 * 1.4f, grid.Radius), pen);
            DrawCaption(g, ip, grid.Radius, "Ellipse");
            ip.X += grid.StepX;

            // Cylinder:
            float radX = grid.Radius / 1.4f;
            float radY = grid.Radius / 6;
            float height = grid.Radius * 1.8f;
            g.DrawEllipse(new RectangleF(ip.X - radX, ip.Y - height / 2, radX * 2, radY * 2), pen);
            g.FillEllipse(new RectangleF(ip.X - radX, ip.Y + height / 2 - radY * 2, radX * 2, radY * 2), Color.FromArgb(fill, pen.Color));
            g.DrawEllipse(new RectangleF(ip.X - radX, ip.Y + height / 2 - radY * 2, radX * 2, radY * 2), pen);
            g.DrawLine(new PointF(ip.X - radX, ip.Y - height / 2 + radY), new PointF(ip.X - radX, ip.Y + height / 2 - radY), pen);
            g.DrawLine(new PointF(ip.X + radX, ip.Y - height / 2 + radY), new PointF(ip.X + radX, ip.Y + height / 2 - radY), pen);
            DrawCaption(g, ip, grid.Radius, "Cylinder");
            ip.X = startIp.X;
            ip.Y += grid.StepY;
            pen.Color = Color.Indigo;

            // Square:
            DrawPolygon(g, ip, grid.Radius, 4, (float)-Math.PI / 4, pen, "Square");
            ip.X += grid.StepX;

            // Rectangle:
            float rectQx = 1.4f;
            float rectQy = 0.6f;
            var rect = new RectangleF(ip.X - grid.Radius * rectQx, ip.Y - grid.Radius * rectQy, grid.Radius * 2 * rectQx, grid.Radius * 2 * rectQy);
            g.DrawRectangle(rect, pen);
            DrawCaption(g, ip, grid.Radius, "Rectangle");
            ip.X += grid.StepX;

            // Cube:
            float cubex = 6;
            var cubePtsFar = DrawPolygon(g, new PointF(ip.X - cubex, ip.Y - cubex), grid.Radius, 4, (float)-Math.PI / 4, pen);
            var cubePtsNear = DrawPolygon(g, new PointF(ip.X + cubex, ip.Y + cubex), grid.Radius, 4, (float)-Math.PI / 4, pen);
            g.DrawLine(cubePtsFar[0], cubePtsNear[0], pen);
            g.DrawLine(cubePtsFar[1], cubePtsNear[1], pen);
            g.DrawLine(cubePtsFar[2], cubePtsNear[2], pen);
            g.DrawLine(cubePtsFar[3], cubePtsNear[3], pen);
            g.FillPolygon(new PointF[] { cubePtsFar[1], cubePtsFar[2], cubePtsNear[2], cubePtsNear[1], }, Color.FromArgb(fill, pen.Color));
            DrawCaption(g, ip, grid.Radius, "Cube");
            ip.X = startIp.X;
            ip.Y += grid.StepY;
            pen.Color = Color.DarkGreen;

            // Pentagon:
            DrawPolygon(g, ip, grid.Radius, 5, (float)-Math.PI / 2, pen, "Pentagon");
            ip.X += grid.StepX;

            // Hexagon:
            // For sample sake, we apply a transform to make the hexagon wider and shorter:
            g.Transform = Matrix3x2.CreateScale(1.4f, 0.8f) * Matrix3x2.CreateTranslation(ip.X, ip.Y);
            DrawPolygon(g, PointF.Empty, grid.Radius, 6, 0, pen, null);
            g.Transform = Matrix3x2.Identity;
            DrawCaption(g, ip, grid.Radius, "Hexagon");
            ip.X += grid.StepX;

            // Octagon:
            DrawPolygon(g, ip, grid.Radius, 8, (float)-Math.PI / 8, pen, "Octagon");
            ip.X = startIp.X;
            ip.Y += grid.StepY;
            pen.Color = Color.DarkRed;

            // Triangle:
            DrawPolygon(g, ip, grid.Radius, 3, (float)-Math.PI / 2, pen, "Triangle");
            ip.X += grid.StepX;

            // Filled pentagram:
            var pts = DrawPolygon(g, ip, grid.Radius, 5, (float)-Math.PI / 2, pen, "Pentagram");
            pts = new PointF[] { pts[0], pts[2], pts[4], pts[1], pts[3], };
            g.FillPolygon(pts, Color.FromArgb(fill, pen.Color));
            g.DrawPolygon(pts, pen);
            ip.X += grid.StepX;

            // Set up a simple kind of oblique projection to draw a pyramid:
            var angle = Math.PI / 6;
            float s = (float)Math.Sin(angle);
            float c = (float)Math.Cos(angle);
            Func<float, float, float, PointF> project = (x_, y_, z_) => new PointF(x_ - c * y_ * 0.5f, -(z_ - s * y_ * 0.5f));
            Func<Vector3, PointF> p3d = v_ => project(v_.X, v_.Y, v_.Z);
            float hedge = grid.Radius; // 1/2 edge
            // Debug - draw the 3 axis:
            /*
            g.DrawLine(project(0, 0, 0), project(100, 0, 0), Color.Red);
            g.DrawLine(project(0, 0, 0), project(0, 100, 0), Color.Green);
            g.DrawLine(project(0, 0, 0), project(0, 0, 100), Color.Blue);
            */
            // 3d points forming a square pyramid:
            var pts3d = new Vector3[]
            {
                new Vector3(-hedge, -hedge, 0),
                new Vector3(hedge, -hedge, 0),
                new Vector3(hedge, hedge, 0),
                new Vector3(-hedge, hedge, 0),
                new Vector3(0, 0, hedge * 2),
            };
            // Project the points to draw the pyramid:
            pts = pts3d.Select(v_ => p3d(v_)).ToArray();
            g.Transform = Matrix3x2.CreateTranslation(ip.X, ip.Y + hedge * 0.7f);
            // Visible edges:
            g.DrawPolygon(new PointF[] { pts[4], pts[1], pts[2], pts[3], pts[4], pts[2] }, pen);
            // Invisible edges:
            pen.Width /= 2;
            pen.Color = Color.FromArgb(fill, pen.Color);
            g.DrawLine(pts[0], pts[4], pen);
            g.DrawLine(pts[0], pts[1], pen);
            g.DrawLine(pts[0], pts[3], pen);
            g.FillPolygon(pts.Take(4).ToArray(), pen.Color);
            //
            g.Transform = Matrix3x2.Identity;
            DrawCaption(g, ip, grid.Radius, "Pyramid");
            ip.X = startIp.X;
            ip.Y += grid.StepY;
            pen.Width *= 2;
            pen.Color = Color.Green;

            // Cone:
            float baseh = grid.Radius * 0.3f;
            pts = DrawPolygon(g, ip, grid.Radius, 3, (float)-Math.PI / 2, null, "Cone");
            g.DrawLines(new PointF[] { pts[2], pts[0], pts[1] }, pen);
            rect = new RectangleF(pts[2].X, pts[2].Y - baseh / 2, pts[1].X - pts[2].X, baseh);
            g.FillEllipse(rect, Color.FromArgb(fill, pen.Color));
            g.DrawEllipse(rect, pen);
            ip.X += grid.StepX;

            // Parallelogram (use graphics.Transform on a rectangle):
            rect = new RectangleF(-grid.Radius * rectQx, -grid.Radius * rectQy, grid.Radius * 2 * rectQx, grid.Radius * 2 * rectQy);
            g.Transform = Matrix3x2.CreateSkew((float)Math.PI / 6, 0) * Matrix3x2.CreateTranslation(ip.X, ip.Y);
            g.DrawRectangle(rect, pen);
            g.Transform = Matrix3x2.Identity;
            DrawCaption(g, ip, grid.Radius, "Parallelogram");
            ip.X += grid.StepX;

            // Trapezoid (use DrawPolygon to just get the points of the square):
            float dx = 10;
            pts = DrawPolygon(g, ip, grid.Radius, 4, (float)-Math.PI / 4, null, "Trapezoid");
            pts[0].X -= dx;
            pts[1].X += dx;
            pts[2].X -= dx;
            pts[3].X += dx;
            g.DrawPolygon(pts, pen);

            // Done:
            doc.Save(stream);
        }
    }
}
