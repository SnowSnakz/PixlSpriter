using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DiscordRPC;
using System.Collections.Generic;
using System.Windows.Shapes;

namespace PixlSpriter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        readonly DiscordRpcClient Discord;
       
        public void Update()
        {
            ToolList.Children.Clear();

            IReadOnlyList<ToolBase> tools = EditorContext.GetToolbox();
            foreach (ToolBase tool in tools)
            {
                ToolList.Children.Add(new ToolIcon(tool, context));
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;

            Discord = new DiscordRpcClient("692569240015994951");
            Discord.Initialize();
            Discord.SetPresence(new RichPresence()
            {
                Assets = new Assets()
                {
                    LargeImageKey = "paintprogram",
                    LargeImageText = "The free pixel art arsenal."
                },
                Timestamps = new Timestamps()
                {
                    Start = DateTime.UtcNow
                },
                State = "Painting with PixlSpriter!"
            });
        }
        EditorContext context;

        ColorWindow PrimaryColorWindow, SecondaryColorWindow;

        private void InitializePanels()
        {
            PrimaryColorWindow = new ColorWindow(true, context);
            SecondaryColorWindow = new ColorWindow(false, context);

            PrimaryColorWindow.Hide();
            SecondaryColorWindow.Hide();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTools();
            InitializeContext();
            InitializePanels();
            UpdateColor();

            Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        public void UpdateColor()
        {
            ColorList.Children.Clear();
            foreach(Color color in context.ColorPalette)
            {
                Border ColorBtn = new Border();
                ColorBtn.MouseLeftButtonDown += ColorBtn_MouseLeftButtonDown;
                ColorBtn.MouseRightButtonDown += ColorBtn_MouseRightButtonDown;
                ColorBtn.Background = new SolidColorBrush(color);
                ColorBtn.Width = 16;
                ColorBtn.Height = 16;
                ColorBtn.MouseWheel += ColorList_MouseWheel;
                ColorBtn.BorderBrush = Brushes.Black;
                ColorList.Children.Add(ColorBtn);
            }
        }

        private void ColorBtn_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                Color c = ((SolidColorBrush)border.Background).Color;
                context.RSecondaryColor = c;
                context.ISecondaryColor = context.ColorPalette.IndexOf(c);
            }
        }

        private void ColorBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(sender is Border border)
            {
                Color c = ((SolidColorBrush)border.Background).Color;
                context.RPrimaryColor = c;
                context.IPrimaryColor = context.ColorPalette.IndexOf(c);
            }
        }

        public void InitializeContext()
        {
            context = new EditorContext
            {
                MainWindow = this,
                CanvasSize = new Size(128, 128),
                RPrimaryColor = Colors.White,
                RSecondaryColor = Colors.Black,
                CurrentLayer = 0,
                EditorControl = Editor,
                PrimaryColorIndexed = false,
                SecondaryColorIndexed = false
            };
            context.ColorPalette.Add(Color.FromRgb(255, 0, 64));
            context.ColorPalette.Add(Color.FromRgb(19, 19, 19));
            context.ColorPalette.Add(Color.FromRgb(27, 27, 27));
            context.ColorPalette.Add(Color.FromRgb(39, 39, 39));
            context.ColorPalette.Add(Color.FromRgb(61, 61, 61));
            context.ColorPalette.Add(Color.FromRgb(93, 93, 93));
            context.ColorPalette.Add(Color.FromRgb(133, 133, 133));
            context.ColorPalette.Add(Color.FromRgb(180, 180, 180));
            context.ColorPalette.Add(Color.FromRgb(255, 255, 255));
            context.ColorPalette.Add(Color.FromRgb(199, 207, 221));
            context.ColorPalette.Add(Color.FromRgb(146, 161, 185));
            context.ColorPalette.Add(Color.FromRgb(101, 115, 146));
            context.ColorPalette.Add(Color.FromRgb(66, 76, 110));
            context.ColorPalette.Add(Color.FromRgb(42, 47, 78));
            context.ColorPalette.Add(Color.FromRgb(26, 25, 50));
            context.ColorPalette.Add(Color.FromRgb(14, 7, 27));
            context.ColorPalette.Add(Color.FromRgb(28, 18, 28));
            context.ColorPalette.Add(Color.FromRgb(57, 31, 33));
            context.ColorPalette.Add(Color.FromRgb(93, 44, 40));
            context.ColorPalette.Add(Color.FromRgb(118, 60, 44));
            context.ColorPalette.Add(Color.FromRgb(138, 72, 54));
            context.ColorPalette.Add(Color.FromRgb(164, 93, 60));
            context.ColorPalette.Add(Color.FromRgb(191, 111, 74));
            context.ColorPalette.Add(Color.FromRgb(230, 156, 105));
            context.ColorPalette.Add(Color.FromRgb(246, 202, 159));
            context.ColorPalette.Add(Color.FromRgb(249, 230, 207));
            context.ColorPalette.Add(Color.FromRgb(237, 171, 80));
            context.ColorPalette.Add(Color.FromRgb(224, 116, 56));
            context.ColorPalette.Add(Color.FromRgb(198, 69, 36));
            context.ColorPalette.Add(Color.FromRgb(142, 37, 29));
            context.ColorPalette.Add(Color.FromRgb(255, 80, 0));
            context.ColorPalette.Add(Color.FromRgb(237, 118, 20));
            context.ColorPalette.Add(Color.FromRgb(255, 162, 20));
            context.ColorPalette.Add(Color.FromRgb(255, 200, 37));
            context.ColorPalette.Add(Color.FromRgb(255, 235, 87));
            context.ColorPalette.Add(Color.FromRgb(211, 252, 126));
            context.ColorPalette.Add(Color.FromRgb(153, 230, 95));
            context.ColorPalette.Add(Color.FromRgb(90, 197, 79));
            context.ColorPalette.Add(Color.FromRgb(51, 152, 75));
            context.ColorPalette.Add(Color.FromRgb(30, 111, 80));
            context.ColorPalette.Add(Color.FromRgb(19, 76, 76));
            context.ColorPalette.Add(Color.FromRgb(12, 46, 68));
            context.ColorPalette.Add(Color.FromRgb(0, 57, 109));
            context.ColorPalette.Add(Color.FromRgb(0, 105, 170));
            context.ColorPalette.Add(Color.FromRgb(0, 152, 220));
            context.ColorPalette.Add(Color.FromRgb(0, 205, 249));
            context.ColorPalette.Add(Color.FromRgb(12, 241, 255));
            context.ColorPalette.Add(Color.FromRgb(148, 253, 255));
            context.ColorPalette.Add(Color.FromRgb(253, 210, 237));
            context.ColorPalette.Add(Color.FromRgb(243, 137, 245));
            context.ColorPalette.Add(Color.FromRgb(219, 63, 253));
            context.ColorPalette.Add(Color.FromRgb(122, 9, 250));
            context.ColorPalette.Add(Color.FromRgb(48, 3, 217));
            context.ColorPalette.Add(Color.FromRgb(12, 2, 147));
            context.ColorPalette.Add(Color.FromRgb(3, 25, 63));
            context.ColorPalette.Add(Color.FromRgb(59, 20, 67));
            context.ColorPalette.Add(Color.FromRgb(98, 36, 97));
            context.ColorPalette.Add(Color.FromRgb(147, 56, 143));
            context.ColorPalette.Add(Color.FromRgb(202, 82, 201));
            context.ColorPalette.Add(Color.FromRgb(200, 80, 134));
            context.ColorPalette.Add(Color.FromRgb(246, 129, 135));
            context.ColorPalette.Add(Color.FromRgb(245, 85, 93));
            context.ColorPalette.Add(Color.FromRgb(234, 50, 60));
            context.ColorPalette.Add(Color.FromRgb(196, 36, 48));
            context.ColorPalette.Add(Color.FromRgb(137, 30, 43));
            context.ColorPalette.Add(Color.FromRgb(87, 28, 39));

            context.ColorPalette.Add(Colors.Black);
            context.SetActiveTool(PencilTool.Instance);
            UpdateLayout();
            Editor.context = context;

            Editor.ImagePanel.Margin = new Thickness(128, 128, 0, 0);
            Pixlmap baseImage = new Pixlmap((int)context.CanvasSize.Width, (int)context.CanvasSize.Height, false);
            Editor.Images.Add(new EditorImageLayer()
            {
                pixlmap = baseImage,
                image = baseImage.GetImage(context)
            });
            Editor.UpdateImageLayers = true;
            Update();
        }

        public void LoadTools()
        {
            new PanTool();
            new PencilTool();
            new LineTool();
        }

        private void ViewToolbox_Checked(object sender, RoutedEventArgs e)
        {
            ToolBox?.Show();
        }
        private void ViewToolbox_Unchecked(object sender, RoutedEventArgs e)
        {
            ToolBox?.Hide();
        }

        private void EditMain(object sender, RoutedEventArgs e)
        {
            if(sender is MenuItem item)
            {
                switch(item.Header)
                {
                    case "Undo":
                        context.ApplyUndo();
                        break;
                    case "Redo":
                        context.ApplyRedo();
                        break;
                    case "Copy":
                    case "Paste":
                    case "Cut":
                    case "Delete":
                        // TODO: Create the selection tool, and implement these features
                        break;
                }
            }
        }

        private void PrimaryColorDisplay_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if(sender is Border border)
            {
                SecondaryColorWindow.Hide();
                PrimaryColorWindow.Show();

                Point windowPoint = border.PointToScreen(new Point(-(PrimaryColorWindow.ActualWidth / 2) + border.ActualWidth / 2, -PrimaryColorWindow.ActualHeight));

                PrimaryColorWindow.ColorCanvas.SelectedColor = context.RPrimaryColor;
                PrimaryColorWindow.Top = windowPoint.Y;
                PrimaryColorWindow.Left = windowPoint.X;
            }
        }

        private void SecondaryColorDisplay_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border)
            {
                PrimaryColorWindow.Hide();
                SecondaryColorWindow.Show();

                Point windowPoint = border.PointToScreen(new Point(-(SecondaryColorWindow.ActualWidth / 2) + border.ActualWidth / 2, -SecondaryColorWindow.ActualHeight));

                SecondaryColorWindow.ColorCanvas.SelectedColor = context.RSecondaryColor;
                SecondaryColorWindow.Top = windowPoint.Y;
                SecondaryColorWindow.Left = windowPoint.X;
            }
        }

        private void ColorList_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double delta = e.Delta / 100;
            foreach(UIElement element in ColorList.Children)
            {
                if(element is Border border)
                {
                    border.Width += delta;
                    border.Height += delta;
                }
            }
        }
    }
}
