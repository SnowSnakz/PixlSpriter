using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PixlSpriter
{
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : UserControl
    {
        public List<EditorImageLayer> Images { get; } = new List<EditorImageLayer>();
        private bool uil = false;
        public bool UpdateImageLayers { get => uil; set { uil = value; if (uil) InvalidateVisual(); } }

        public EditorContext context;

        double zoom = 1;
        public static void PanEditor(EditorContext context, Vector offset)
        {
            Thickness margin = context.EditorControl.ImagePanel.Margin;
            margin.Left += offset.X;
            margin.Top += offset.Y;
            context.EditorControl.ImagePanel.Margin = margin;
        }

        public Editor()
        {
            InitializeComponent();
            Application.Current.MainWindow.KeyUp += Grid_KeyUp;
            Application.Current.MainWindow.KeyDown += Grid_KeyDown;
        }

        private void EventBase()
        {
            Cursor = context.ActiveTool.Cursor;
            InvalidateVisual();
        }

        public delegate void RenderEvent(EditorContext editorContext, DrawingContext drawingContext);
        public event RenderEvent Render;

        bool isPanning = false;
        MouseButton lastPanningButton;
        Point lastPanningPos = new Point(-1, -1);

        public Point MouseToPixelCoords(EditorContext context, MouseEventArgs args)
        {
            Point point = args.GetPosition(this);

            point.X -= ImagePanel.Margin.Left;
            point.Y -= ImagePanel.Margin.Top;

            point.X *= context.CanvasSize.Width / ImagePanel.Width;
            point.Y *= context.CanvasSize.Height / ImagePanel.Height;

            point.X = Math.Floor(point.X);
            point.Y = Math.Floor(point.Y);

            return point;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            EventBase();

            if (!context.ActiveTool.DisablePanning)
            {
                if(!context.ActiveTool.ReserveMiddleClick)
                {
                    isPanning = e.ChangedButton == MouseButton.Middle;
                    lastPanningButton = e.ChangedButton;
                    lastPanningPos = e.GetPosition(this);
                }
                if (!context.ActiveTool.ReserveSpaceKey)
                {
                    if(Keyboard.IsKeyDown(Key.Space))
                    {
                        isPanning = true;
                        lastPanningButton = e.ChangedButton;
                        lastPanningPos = e.GetPosition(this);
                    }
                }

                if (isPanning)
                {
                    return;
                }
            }

            context.ActiveTool.MouseDown(context, e);
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            EventBase();
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            EventBase();
            Grid_MouseUp(sender, new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Left));
            Grid_MouseUp(sender, new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Right));
            Grid_MouseUp(sender, new MouseButtonEventArgs(e.MouseDevice, e.Timestamp, MouseButton.Middle));
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            EventBase();

            if (isPanning && e.ChangedButton == lastPanningButton)
            {
                isPanning = false;
            }

            context.ActiveTool.MouseUp(context, e);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            EventBase();

            Point mp = e.GetPosition(this);

            if (isPanning)
            {
                if(context.ActiveTool.DisablePanning)
                {
                    isPanning = false;
                }
                else
                {
                    PanEditor(context, Point.Subtract(mp, lastPanningPos));
                }
            }

            lastPanningPos = mp;

            context.ActiveTool.MouseMove(context, e);
        }

        private void Grid_StylusDown(object sender, StylusDownEventArgs e)
        {
            EventBase();
            context.ActiveTool.StylusDown(context, e);
        }

        private void Grid_StylusButtonDown(object sender, StylusButtonEventArgs e)
        {
            EventBase();
            context.ActiveTool.StylusButtonDown(context, e);
        }

        private void Grid_StylusButtonUp(object sender, StylusButtonEventArgs e)
        {
            EventBase();
            context.ActiveTool.StylusButtonUp(context, e);
        }

        private void Grid_StylusEnter(object sender, StylusEventArgs e)
        {
            EventBase();
        }

        private void Grid_StylusMove(object sender, StylusEventArgs e)
        {
            EventBase();
            context.ActiveTool.StylusMove(context, e);
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            EventBase();
            context.ActiveTool.KeyDown(context, e);
        }

        private void Grid_KeyUp(object sender, KeyEventArgs e)
        {
            EventBase();
            context.ActiveTool.KeyUp(context, e);
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                if (e.Key == Key.Z)
                {
                    context.ApplyUndo();
                }
                else if (e.Key == Key.Y)
                {
                    context.ApplyRedo();
                }
            }
        }

        private void Grid_StylusUp(object sender, StylusEventArgs e)
        {
            EventBase();
            context.ActiveTool.StylusUp(context, e);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            ImagePanelBorder.Width = ImagePanel.Width + 2;
            ImagePanelBorder.Height = ImagePanel.Height + 2;

            Thickness margin = ImagePanel.Margin;
            margin.Left -= 1;
            margin.Top -= 1;
            margin.Right += 1;
            margin.Bottom += 1;

            ImagePanelBorder.Margin = margin;

            if(UpdateImageLayers)
            {
                UpdateImageLayers = false;

                ImagePanel.Children.Clear();
                foreach (EditorImageLayer layer in Images)
                {
                    if(layer.control != null)
                    {
                        ImagePanel.Children.Add(layer.control);
                    }
                }
            }

            base.OnRender(drawingContext);
            Render?.Invoke(context, drawingContext);
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zi = (double)e.Delta / 340;

            if (zoom + zi < 0.25 || zoom + zi > 6) return; 

            zoom += zi;

            Point p1 = e.GetPosition(this);
            Point point = p1;

            point.X -= ImagePanel.Margin.Left;
            point.Y -= ImagePanel.Margin.Top;

            point.X *= context.CanvasSize.Width / ImagePanel.Width;
            point.Y *= context.CanvasSize.Height / ImagePanel.Height;

            point.X = Math.Floor(point.X);
            point.Y = Math.Floor(point.Y);
            ImagePanel.Margin = new Thickness(p1.X - (point.X * zoom), p1.Y - (point.Y * zoom), 0, 0);
            

            ImagePanel.Width = context.CanvasSize.Width * zoom;
            ImagePanel.Height = context.CanvasSize.Width * zoom;

            EventBase();
        }
    }
}
