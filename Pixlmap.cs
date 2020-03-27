using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace PixlSpriter
{
    public class Pixlmap
    {
        byte[] Pixels;
        int Width, Height;

        bool Indexed;

        public Pixlmap(int width, int height, bool indexed)
        {
            SetSize(width, height, indexed);
        }

        public Pixlmap(Pixlmap other)
        {
            Pixels = (byte[])other.Pixels.Clone();
            Width = other.Width;
            Height = other.Height;
            Indexed = other.Indexed;
        }

        public ImageSource GetImage(EditorContext context)
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);

            PixelFormat format;
            BitmapPalette palette = null;

            if(Indexed)
            {
                palette = new BitmapPalette(context.ColorPalette);
                format = PixelFormats.Indexed8;
            }
            else
            {
                format = PixelFormats.Bgra32;
            }

            int bpp = format.BitsPerPixel;
            int bypp = (bpp + 7) / 8;
            int stride = 4 * ((Width * bypp + 3) / 4);

            WriteableBitmap bmp = new WriteableBitmap(Width, Height, g.DpiX, g.DpiY, format, palette);
            bmp.Lock();
            bmp.WritePixels(new Int32Rect(0, 0, Width, Height), Pixels, stride, 0);
            bmp.Unlock();

            return bmp;
        }

        public void SetPixel(int xx, int yy, int cval)
        {
            if (xx < 0 || xx >= Width || yy < 0 || yy >= Height) return;

            if (Indexed) Pixels[yy * Width + xx] = (byte)cval;
            else
            {
                Pixels[yy * Width * 4 + xx * 4] = (byte)(cval & 0xFF);
                Pixels[yy * Width * 4 + xx * 4 + 1] = (byte)((cval >> 8) & 0xFF);
                Pixels[yy * Width * 4 + xx * 4 + 2] = (byte)((cval >> 16) & 0xFF);
                Pixels[yy * Width * 4 + xx * 4 + 3] = (byte)((cval >> 24) & 0xFF);
            }
        }

        public void DrawPointSquare(Point point, int radius, int cval)
        {
            if (radius <= 0) return;

            int x = (int)point.X, y = (int)point.Y;

            if (radius == 1)
            {
                SetPixel(x, y, cval);
                return;
            }

            int nr = radius / 2;

            for (int xx = x - nr; xx < x + nr; xx++)
            {
                for (int yy = y - nr; yy < y + nr; yy++)
                {
                    if (xx < 0 || yy < 0 || xx > Width || yy > Height) continue;
                    SetPixel(xx, yy, cval);
                }
            }
        }

        public void DrawPointCircle(Point point, int radius, int cval)
        {
            if (radius <= 0) return;

            int x = (int)point.X, y = (int)point.Y;

            if (radius == 1)
            {
                SetPixel(x, y, cval);
                return;
            }
            
            double nr = radius / 2;

            for (int xx = (int)Math.Ceiling(x - nr); xx < x + nr; xx++)
            {
                for(int yy = (int)Math.Ceiling(y - nr); yy < y + nr; yy++)
                {
                    if (xx < 0 || yy < 0 || xx >= Width || yy >= Height) continue;
                    if (new Vector(xx - x, yy - y).LengthSquared <= radius) SetPixel(xx, yy, cval);
                }
            }
        }

        private int ColorToCVal(EditorContext context, Color color)
        {
            if (Indexed)
            {
                if (context.ColorPalette.Contains(color))
                {
                    return context.ColorPalette.IndexOf(color);
                }
                else
                {
                    int closest = 0;
                    int rg = 1000;
                    int rb = 1000;
                    int ra = 1000;
                    int rr = 1000;

                    for (int i = 0; i < context.ColorPalette.Count; i++)
                    {
                        Color c = context.ColorPalette[i];

                        int tg, tb, ta, tr;
                        if ((tr = Math.Abs(c.R - color.R)) < rr && (tg = Math.Abs(c.G - color.G)) < rg && (tb = Math.Abs(c.B - color.B)) < rb && (ta = Math.Abs(c.A - color.A)) < ra)
                        {
                            closest = i;
                            rg = tg;
                            rb = tb;
                            ra = ta;
                            rr = tr;
                        }
                    }

                    return closest;
                }
            }
            else
            {
                return (color.B | (color.G << 8) | (color.R << 16) | (color.A << 24));
            }
        }

        public void DrawPointSquare(EditorContext context, Point point, int radius, Color val)
        {
            int cval = ColorToCVal(context, val);
            DrawPointSquare(point, radius, cval);
        }
        public void DrawPointCircle(EditorContext context, Point point, int radius, Color val)
        {
            int cval = ColorToCVal(context, val);
            DrawPointCircle(point, radius, cval);
        }
        public void DrawLineSquare(EditorContext context, Point a, Point b, int radius, Color val)
        {
            int cval = ColorToCVal(context, val);
            DrawLineSquare(a, b, radius, cval);
        }
        public void DrawLineCircle(EditorContext context, Point a, Point b, int radius, Color val)
        {
            int cval = ColorToCVal(context, val);
            DrawLineCircle(a, b, radius, cval);
        }
        public void DrawLineSquare(Point a, Point b, int radius, int cval)
        {
            int x0 = (int)a.X, y0 = (int)a.Y;
            int x1 = (int)b.X, y1 = (int)b.Y;

            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            for (; ; )
            {
                DrawPointSquare(new Point(x0, y0), radius, cval);
                if (x0 == x1 && y0 == y1) break;
                e2 = err;
                if (e2 > -dx) { err -= dy; x0 += sx; }
                if (e2 < dy) { err += dx; y0 += sy; }
            }
        }
        public void DrawLineCircle(Point a, Point b, int radius, int cval)
        {
            int x0 = (int)a.X, y0 = (int)a.Y;
            int x1 = (int)b.X, y1 = (int)b.Y;

            int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2, e2;
            for (; ; )
            {
                DrawPointCircle(new Point(x0, y0), radius, cval);
                if (x0 == x1 && y0 == y1) break;
                e2 = err;
                if (e2 > -dx) { err -= dy; x0 += sx; }
                if (e2 < dy) { err += dx; y0 += sy; }
            }
        }
        public void DrawPathSquare(PointCollection points, int lineWidth, int cval)
        {
            if (points.Count <= 0) return;
            if (points.Count == 1) DrawPointSquare(points[0], lineWidth, cval);

            for(int i = 1; i < points.Count; i++)
            {
                DrawLineSquare(points[i - 1], points[i], lineWidth, cval);
            }
        }
        public void DrawPathCircle(PointCollection points, int lineWidth, int cval)
        {
            if (points.Count <= 0) return;
            if (points.Count == 1) DrawPointSquare(points[0], lineWidth, cval);

            for (int i = 1; i < points.Count; i++)
            {
                DrawLineCircle(points[i - 1], points[i], lineWidth, cval);
            }
        }
        public void DrawPerfectPathSquare(PointCollection points, int lineWidth, int cval)
        {
            if (points.Count <= 0) return;
            if (points.Count == 1) DrawPointSquare(points[0], lineWidth, cval);

            Point prev = points[0];

            for (int i = 1; i < points.Count; i++)
            {
                bool avoid = false;

                if ((i + 1) < points.Count)
                {
                    avoid = (prev.X == points[i].X || prev.Y == points[i].Y);
                    avoid &= (points[i + 1].X == points[i].X || points[i + 1].Y == points[i].Y);
                    avoid &= points[i + 1].X != prev.X;
                    avoid &= points[i + 1].Y != prev.Y;
                }

                if (!avoid)
                {
                    DrawLineSquare(prev, points[i], lineWidth, cval);
                    prev = points[i];
                }
            }
        }
        public void DrawPerfectPathCircle(PointCollection points, int lineWidth, int cval)
        {
            if (points.Count <= 0) return;
            if (points.Count == 1) DrawPointSquare(points[0], lineWidth, cval);

            Point prev = points[0];

            for (int i = 1; i < points.Count; i++)
            {
                bool avoid = false;

                if ((i + 1) < points.Count)
                {
                    avoid = ((prev.X == points[i].X || prev.Y == points[i].Y)
                        && (points[i + 1].X == points[i].X || points[i + 1].Y == points[i].Y)
                        && points[i + 1].X != prev.X
                        && points[i + 1].Y != prev.Y) || points[i] == prev;
                }

                if (!avoid)
                {
                    DrawLineCircle(prev, points[i], lineWidth, cval);
                    prev = points[i];
                }
            }
        }
        public void DrawPathSquare(EditorContext context, PointCollection points, int lineWidth, Color val)
        {
            int cval = ColorToCVal(context, val);
            DrawPathSquare(points, lineWidth, cval);
        }
        public void DrawPathCircle(EditorContext context, PointCollection points, int lineWidth, Color val)
        {
            int cval = ColorToCVal(context, val);
            DrawPathCircle(points, lineWidth, cval);
        }
        public void DrawPerfectPathSquare(EditorContext context, PointCollection points, int lineWidth, Color val)
        {
            int cval = ColorToCVal(context, val);
            DrawPerfectPathSquare(points, lineWidth, cval);
        }
        public void DrawPerfectPathCircle(EditorContext context, PointCollection points, int lineWidth, Color val)
        {
            int cval = ColorToCVal(context, val);
            DrawPerfectPathCircle(points, lineWidth, cval);
        }

        public void Clear()
        {
            SetSize(Width, Height, Indexed);
        }

        public void SetSize(int width, int height, bool indexed)
        {
            Width = width;
            Height = height;
            if (indexed) Pixels = new byte[width * height];
            else Pixels = new byte[width * 4 * height];
        }

        public byte[] GetRectangle(Int32Rect rectangle)
        {
            int x = rectangle.X;
            int y = rectangle.Y;

            int size = Width;
            if (!Indexed) size *= 4;
            size *= Height;

            byte[] rect = new byte[size];

            for(int i = 0; i < rectangle.Width; i++)
            {
                for(int j = 0; j < rectangle.Height; j++)
                {
                    int xx = x + i, yy = y + j;
                    if (xx < 0 || xx >= Width || yy < 0 || yy >= Width) continue;

                    if(Indexed)
                    {
                        rect[j * Width + i] = Pixels[yy * Width + xx];
                    }
                    else
                    {
                        rect[j * Width * 4 + i * 4] = Pixels[yy * Width * 4 + xx * 4];
                        rect[j * Width * 4 + i * 4 + 1] = Pixels[yy * Width * 4 + xx* 4 + 1];
                        rect[j * Width * 4 + i * 4 + 2] = Pixels[yy * Width * 4 + xx * 4 + 2];
                        rect[j * Width * 4 + i * 4 + 3] = Pixels[yy* Width * 4 + xx * 4 + 3];
                    }
                }
            }

            return rect;
        }
        public void SetRectangle(byte[] arr, Int32Rect rectangle)
        {
            int x = rectangle.X;
            int y = rectangle.Y;

            for (int i = 0; i < rectangle.Width; i++)
            {
                for (int j = 0; j < rectangle.Height; j++)
                {
                    int xx = x + i, yy = y + j;
                    if (xx < 0 || xx >= Width || yy < 0 || yy >= Width) continue;

                    if (Indexed)
                    {
                        Pixels[j * Width + i] = arr[yy * Width + xx];
                    }
                    else
                    {
                        Pixels[yy * Width * 4 + xx * 4] = arr[j * Width * 4 + i * 4];
                        Pixels[yy * Width * 4 + xx * 4 + 1] = arr[j * Width * 4 + i * 4 + 1];
                        Pixels[yy * Width * 4 + xx * 4 + 2] = arr[j * Width * 4 + i * 4 + 2];
                        Pixels[yy * Width * 4 + xx * 4 + 3] = arr[j * Width * 4 + i * 4 + 3];
                    }
                }
            }
        }
    }
}
