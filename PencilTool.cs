using PixlSpriter.Undo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace PixlSpriter
{
    public class PencilTool : ToolBase
    {
        public static PencilTool Instance { get; private set; }

        CheckBox UI_BrushIsSquare;
        CheckBox UI_PixelPerfect;
        TextBox  UI_BrushSize;
        TextBlock UI_BrushSizeLabel;

        public PencilTool() {
            previewLayer = new EditorImageLayer
            {
                pixlmap = new Pixlmap(1, 1, true)
            };
            EditorContext.Toolbox = this;
            Instance = this;

            UI_BrushIsSquare = new CheckBox();
            UI_BrushIsSquare.IsChecked = true;
            UI_BrushIsSquare.Checked += UI_BrushIsSquare_Checked;
            UI_BrushIsSquare.Unchecked += UI_BrushIsSquare_Unchecked;
            UI_BrushIsSquare.Content = new TextBlock() { Text = "Circle Brush" };
            UI_PixelPerfect = new CheckBox();
            UI_PixelPerfect.IsChecked = true;
            UI_PixelPerfect.Checked += UI_PixelPerfect_Checked;
            UI_PixelPerfect.Unchecked += UI_PixelPerfect_Unchecked;
            UI_PixelPerfect.Content = new TextBlock() { Text = "Pixel Perfect" };
            UI_BrushSize = new TextBox();
            UI_BrushSize.PreviewTextInput += UI_BrushSize_PreviewTextInput;
            UI_BrushSize.MouseWheel += UI_BrushSize_MouseWheel;
            UI_BrushSize.Text = "1";
            UI_BrushSize.VerticalAlignment = VerticalAlignment.Center;
            UI_BrushSizeLabel = new TextBlock() { Text = "Brush Size", TextAlignment = TextAlignment.Right, Padding = new Thickness(0, 0, 8, 0), VerticalAlignment = VerticalAlignment.Center };

            OptionsPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(128, GridUnitType.Star) });
            OptionsPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(128, GridUnitType.Star) });
            OptionsPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(32, GridUnitType.Pixel) });
            OptionsPanel.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(32, GridUnitType.Pixel) });

            OptionsPanel.Children.Add(UI_BrushIsSquare);
            OptionsPanel.Children.Add(UI_PixelPerfect);
            OptionsPanel.Children.Add(UI_BrushSize);
            OptionsPanel.Children.Add(UI_BrushSizeLabel);

            Grid.SetColumn(UI_BrushIsSquare, 0);
            Grid.SetRow(UI_BrushIsSquare, 0);
            Grid.SetColumn(UI_PixelPerfect, 1);
            Grid.SetRow(UI_PixelPerfect, 0);
            Grid.SetColumn(UI_BrushSizeLabel, 0);
            Grid.SetRow(UI_BrushSizeLabel, 1);
            Grid.SetColumn(UI_BrushSize, 1);
            Grid.SetRow(UI_BrushSize, 1);
        }

        private void UI_PixelPerfect_Unchecked(object sender, RoutedEventArgs e)
        {
            pixelPerfect = false;
        }

        private void UI_PixelPerfect_Checked(object sender, RoutedEventArgs e)
        {
            pixelPerfect = true;
        }

        private void UI_BrushSize_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            brushWidth += e.Delta / 120;
            UI_BrushSize.Text = brushWidth.ToString();
        }

        private void UI_BrushSize_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            for(int i = 0; i < e.Text.Length; i++)
            {
                char c = e.Text[i];
                if(!char.IsDigit(c))
                {
                    e.Text.Remove(i, 1);
                }
            }
            e.Handled = true;
        }

        private void UI_BrushIsSquare_Unchecked(object sender, RoutedEventArgs e)
        {
            brushIsSquare = true;
        }

        private void UI_BrushIsSquare_Checked(object sender, RoutedEventArgs e)
        {
            brushIsSquare = false;
        }

        public override string Name => "Pencil Tool";
        public override string Description => "Hold and drag to freedraw";
        private ImageSource IconSource = Helper.GetResourceImage("res/tools/penciltool.png");
        public override ImageSource Icon => IconSource;

        EditorImageLayer previewLayer = new EditorImageLayer();

        Color drawColor;
        bool useIndexColor = false;
        int indexColor = 0;
        bool isPressed = false;
        int brushWidth = 1;
        bool pixelPerfect = true;
        bool brushIsSquare = false;

        Point previousPoint = new Point(-1, -1);

        PointCollection points = new PointCollection();

        MouseButton lastButton;

        public override void MouseDown(EditorContext context, MouseButtonEventArgs args)
        {
            if(!isPressed)
            {
                points.Clear();
                switch(args.ChangedButton)
                {
                    case MouseButton.Left:
                        drawColor = context.PrimaryColorIndexed ? context.ColorPalette[context.IPrimaryColor] : context.RPrimaryColor;
                        indexColor = context.IPrimaryColor;
                        useIndexColor = context.PrimaryColorIndexed;
                        isPressed = true;
                        previousPoint = new Point(-1, -1);
                        points.Add(context.EditorControl.MouseToPixelCoords(context, args)); 
                        previewLayer.pixlmap.SetSize((int)context.CanvasSize.Width, (int)context.CanvasSize.Height, useIndexColor);
                        previewLayer.image = previewLayer.pixlmap.GetImage(context);
                        context.EditorControl.Images.Insert(context.CurrentLayer + 1, previewLayer);
                        context.EditorControl.UpdateImageLayers = true;
                        break;
                    case MouseButton.Right:
                        drawColor = context.SecondaryColorIndexed ? context.ColorPalette[context.ISecondaryColor] : context.RSecondaryColor;
                        indexColor = context.ISecondaryColor;
                        useIndexColor = context.SecondaryColorIndexed;
                        isPressed = true;
                        previousPoint = new Point(-1, -1);
                        points.Add(context.EditorControl.MouseToPixelCoords(context, args));
                        previewLayer.pixlmap.SetSize((int)context.CanvasSize.Width, (int)context.CanvasSize.Height, useIndexColor);
                        previewLayer.image = previewLayer.pixlmap.GetImage(context);
                        context.EditorControl.Images.Insert(context.CurrentLayer + 1, previewLayer);
                        context.EditorControl.UpdateImageLayers = true;
                        break;
                    case MouseButton.Middle:
                        PanTool.Instance.MouseDown(context, args);
                        break;
                }
            }
        }

        public override void MouseUp(EditorContext context, MouseButtonEventArgs args)
        {
            if(isPressed)
            {
                isPressed = false;
                context.EditorControl.Images.Remove(previewLayer);

                if (points.Count <= 0) return;

                EditorImageLayer lyr = context.EditorControl.Images[context.CurrentLayer];

                void ContinueWithAction()
                {
                    if (pixelPerfect)
                    {
                        if (brushIsSquare)
                        {
                            if (useIndexColor) lyr.pixlmap.DrawPerfectPathSquare(points, brushWidth, indexColor);
                            else lyr.pixlmap.DrawPerfectPathSquare(context, points, brushWidth, drawColor);
                        }
                        else
                        {
                            if (useIndexColor) lyr.pixlmap.DrawPerfectPathCircle(points, brushWidth, indexColor);
                            else lyr.pixlmap.DrawPerfectPathCircle(context, points, brushWidth, drawColor);
                        }
                    }
                    else
                    {
                        if (brushIsSquare)
                        {
                            if (useIndexColor) lyr.pixlmap.DrawPathSquare(points, brushWidth, indexColor);
                            else lyr.pixlmap.DrawPathSquare(context, points, brushWidth, drawColor);
                        }
                        else
                        {
                            if (useIndexColor) lyr.pixlmap.DrawPathCircle(points, brushWidth, indexColor);
                            else lyr.pixlmap.DrawPathCircle(context, points, brushWidth, drawColor);
                        }
                    }

                    lyr.image = lyr.pixlmap.GetImage(context);
                    context.EditorControl.UpdateImageLayers = true;
                }

                int minX = (int)points[0].X, minY = (int)points[0].Y, maxX = (int)points[0].X, maxY = (int)points[0].Y;

                foreach(Point p in points)
                {
                    if (p.X < minX) minX = (int)p.X;
                    if (p.Y < minY) minY = (int)p.Y;
                    if (p.X > maxX) maxX = (int)p.X;
                    if (p.Y > maxY) maxY = (int)p.Y;
                }

                Int32Rect rect = new Int32Rect(Math.Max(minX - brushWidth * 2, 0), Math.Max(minY - brushWidth * 2, 0), (int)Math.Min(maxX - minX + brushWidth * 4, context.CanvasSize.Width), (int)Math.Min(maxY - minY + brushWidth * 4, context.CanvasSize.Width));

                ChangePartUndo undo = new ChangePartUndo(lyr, rect, ContinueWithAction);

                context.AddUndo(undo);
            }
        }

        public override void MouseMove(EditorContext context, MouseEventArgs args)
        {
            if (isPressed)
            {
                Point point = context.EditorControl.MouseToPixelCoords(context, args);

                if (point == previousPoint) return;

                points.Add(point);
                previousPoint = point;

                previewLayer.pixlmap.Clear();

                if (pixelPerfect)
                {
                    if (brushIsSquare)
                    {
                        if (useIndexColor) previewLayer.pixlmap.DrawPerfectPathSquare(points, brushWidth, indexColor);
                        else previewLayer.pixlmap.DrawPerfectPathSquare(context, points, brushWidth, drawColor);
                    }
                    else
                    {
                        if (useIndexColor) previewLayer.pixlmap.DrawPerfectPathCircle(points, brushWidth, indexColor);
                        else previewLayer.pixlmap.DrawPerfectPathCircle(context, points, brushWidth, drawColor);
                    }
                }
                else
                {
                    if (brushIsSquare)
                    {
                        if (useIndexColor) previewLayer.pixlmap.DrawPathSquare(points, brushWidth, indexColor);
                        else previewLayer.pixlmap.DrawPathSquare(context, points, brushWidth, drawColor);
                    }
                    else
                    {
                        if (useIndexColor) previewLayer.pixlmap.DrawPathCircle(points, brushWidth, indexColor);
                        else previewLayer.pixlmap.DrawPathCircle(context, points, brushWidth, drawColor);
                    }
                }

                previewLayer.image = previewLayer.pixlmap.GetImage(context);
            }
        }

        public override void KeyDown(EditorContext context, KeyEventArgs args)
        {
        }
        public override void KeyUp(EditorContext context, KeyEventArgs args)
        {
        }
    }
}
