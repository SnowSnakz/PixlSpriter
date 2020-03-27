using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        public ToolBase ActiveTool { get; private set; }

        public Size CanvasSize { get; set; }

        private List<IUndoable> UndoHistory { get; } = new List<IUndoable>();

        public Editor EditorControl { get; set; }
        public MainWindow MainWindow { get; set; }

        public List<Color> ColorPalette { get; } = new List<Color>();

        private Color prcolor;
        private Color srcolor;
        private int picolor;
        private int sicolor;

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
                picolor = value;
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
        public int ISecondaryColor {
            get => sicolor;
            set
            {
                sicolor = value;
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
