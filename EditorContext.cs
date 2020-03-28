using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PixlSpriter
{
    public class EditorContext
    {
        private static readonly List<ToolBase> Tools = new List<ToolBase>();
        public static ToolBase Toolbox 
        { 
            set {
                if (!Tools.Contains(value))Tools.Add(value); 
            }
        }

        public static IReadOnlyList<ToolBase> GetToolbox()
        {
            return Tools.AsReadOnly();
        }

        public EventList<EditorImageLayer> Layers { get; } = new EventList<EditorImageLayer>();

        public ToolBase ActiveTool { get; private set; }

        public Size CanvasSize { get; set; }

        private List<IUndoable> UndoHistory { get; } = new List<IUndoable>();

        public Editor EditorControl { get; set; }
        public MainWindow MainWindow { get; set; }

        public List<Color> ColorPalette { get; } = new List<Color>();

        private Color prcolor;
        private Color srcolor;
        private int picolor = -1;
        private int sicolor = -1;

        public Color RPrimaryColor {
            get => prcolor; 
            set { 
                prcolor = value; 
                MainWindow.PrimaryColorDisplay.Background = new SolidColorBrush(prcolor);
                MainWindow.PrimaryColorText.Text = $"{prcolor}";

                if (prcolor.A <= 128)
                {
                    MainWindow.PrimaryColorText.Foreground = Brushes.White;
                    return;
                }

                int avg = (prcolor.R + prcolor.G + prcolor.B) / 3;

                if(avg >= 128) MainWindow.PrimaryColorText.Foreground = Brushes.Black;
                else MainWindow.PrimaryColorText.Foreground = Brushes.White;
            } 
        }
        public Color RSecondaryColor {
            get => srcolor;
            set
            {
                srcolor = value;
                MainWindow.SecondaryColorDisplay.Background = new SolidColorBrush(srcolor);
                MainWindow.SecondaryColorText.Text = $"{srcolor}";

                if (srcolor.A <= 128)
                {
                    MainWindow.SecondaryColorText.Foreground = Brushes.White;
                    return;
                }

                int avg = (srcolor.R + srcolor.G + srcolor.B) / 3;

                if (avg >= 128) MainWindow.SecondaryColorText.Foreground = Brushes.Black;
                else MainWindow.SecondaryColorText.Foreground = Brushes.White;
            }
        }
        public int IPrimaryColor
        {
            get => picolor;
            set
            {
                if (picolor >= 0)
                {
                    if (MainWindow.ColorList.Children[picolor] is Border border)
                    {
                        if (sicolor == picolor)
                        {
                            border.BorderBrush = Brushes.Black;
                        }
                        else border.BorderThickness = new Thickness(0, 0, 0, 0);
                    }
                }

                if (value >= 0)
                {
                    picolor = value;

                    if (MainWindow.ColorList.Children[picolor] is Border border)
                    {
                        if (sicolor == picolor) border.BorderBrush = new LinearGradientBrush(Colors.White, Colors.Black, 90);
                        else border.BorderBrush = Brushes.White;

                        border.BorderThickness = new Thickness(2, 2, 2, 2);
                    }

                    Color c = ColorPalette[picolor];
                    MainWindow.PrimaryColorDisplay.Background = new SolidColorBrush(c);

                    MainWindow.PrimaryColorText.Text = $"Index {picolor}";

                    if (c.A <= 128)
                    {
                        MainWindow.PrimaryColorText.Foreground = Brushes.White;
                        return;
                    }

                    int avg = (c.R + c.G + c.B) / 3;

                    if (avg >= 128) MainWindow.PrimaryColorText.Foreground = Brushes.Black;
                    else MainWindow.PrimaryColorText.Foreground = Brushes.White;
                }
            }
        }
        public int ISecondaryColor
        {
            get => sicolor;
            set
            {
                if (sicolor >= 0)
                {
                    if (MainWindow.ColorList.Children[sicolor] is Border border)
                    {
                        if (sicolor == picolor)
                        {
                            border.BorderBrush = Brushes.White;
                        }
                        else border.BorderThickness = new Thickness(0, 0, 0, 0);
                    }
                }

                if (value >= 0)
                {
                    sicolor = value;

                    if (MainWindow.ColorList.Children[sicolor] is Border border)
                    {
                        if (sicolor == picolor) border.BorderBrush = new LinearGradientBrush(Colors.White, Colors.Black, 90);
                        else border.BorderBrush = Brushes.Black;

                        border.BorderThickness = new Thickness(2, 2, 2, 2);
                    }

                    Color c = ColorPalette[sicolor];
                    MainWindow.SecondaryColorDisplay.Background = new SolidColorBrush(c);
                    MainWindow.SecondaryColorText.Text = $"Index {sicolor}";

                    if (c.A <= 128)
                    {
                        MainWindow.SecondaryColorText.Foreground = Brushes.White;
                        return;
                    }

                    int avg = (c.R + c.G + c.B) / 3;

                    if (avg >= 128) MainWindow.SecondaryColorText.Foreground = Brushes.Black;
                    else MainWindow.SecondaryColorText.Foreground = Brushes.White;
                }
            }
        }

        public bool PrimaryColorIndexed { get; set; }
        public bool SecondaryColorIndexed { get; set; }

        public int CurrentLayer { get; set; }
        public int UndoCurrent { get; private set; }

        public void AddUndo(IUndoable undo)
        {
            if(UndoCurrent != UndoHistory.Count)
            {
                UndoHistory.RemoveRange(UndoCurrent, UndoHistory.Count - UndoCurrent);
            }
            UndoCurrent++;
            UndoHistory.Add(undo);
        }

        public void ApplyUndo()
        {
            if (UndoCurrent <= 0) return;
            UndoCurrent--;
            UndoHistory[UndoCurrent].undo(this);
        }
        public void ApplyRedo()
        {
            if (UndoCurrent >= UndoHistory.Count) return;
            UndoHistory[UndoCurrent].redo(this); 
            UndoCurrent++;
        }

        public void SetActiveTool(ToolBase tool)
        {
            ActiveTool?.Deactivated(this);
            ActiveTool = tool;
            ActiveTool?.Activated(this);
        }
    }
}
