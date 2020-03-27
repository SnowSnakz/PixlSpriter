using PixlSpriter.Undo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace PixlSpriter
{
    public class LineTool : ToolBase
    {
        public static LineTool Instance { get; private set; }

        public override string Name => "Line Tool";

        public override string Description => "Click and then drag to another location to draw a straight line.";

        private ImageSource IconSource = Helper.GetResourceImage("res/tools/linetool.png");
        public override ImageSource Icon => IconSource;

        EditorImageLayer previewLayer = new EditorImageLayer();

        Color drawColor;
        int indexColor;
        bool isPressed;
        Point previousPoint;
        bool useIndexColor;
        int brushWidth = 1;
        bool brushIsSquare = false;

        public LineTool()
        {
            Instance = this;
            EditorContext.Toolbox = this;
            previewLayer = new EditorImageLayer()
            {
                pixlmap = new Pixlmap(1, 1, true)
            };
        }

        public override void MouseDown(EditorContext context, MouseButtonEventArgs args)
        {
            if (!isPressed)
            {
                Point mousePos = context.EditorControl.MouseToPixelCoords(context, args);

                switch (args.ChangedButton)
                {
                    case MouseButton.Left:
                        drawColor = context.PrimaryColorIndexed ? context.ColorPalette[context.IPrimaryColor] : context.RPrimaryColor;
                        indexColor = context.IPrimaryColor;
                        useIndexColor = context.PrimaryColorIndexed;
                        isPressed = true;
                        previousPoint = mousePos;
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
                        previousPoint = mousePos;
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
            if (isPressed)
            {
                isPressed = false;
                context.EditorControl.Images.Remove(previewLayer);

                Point mousePosition = context.EditorControl.MouseToPixelCoords(context, args);

                EditorImageLayer lyr = context.EditorControl.Images[context.CurrentLayer];

                int minX = (int)Math.Min(previousPoint.X, mousePosition.X), maxX = (int)Math.Max(previousPoint.X, mousePosition.X);
                int minY = (int)Math.Min(previousPoint.Y, mousePosition.Y), maxY = (int)Math.Max(previousPoint.Y, mousePosition.Y);

                void ContinueWithAction()
                {
                    if (brushIsSquare)
                    {
                        if (useIndexColor) lyr.pixlmap.DrawLineSquare(previousPoint, mousePosition, brushWidth, indexColor);
                        else lyr.pixlmap.DrawLineSquare(context, previousPoint, mousePosition, brushWidth, drawColor);
                    }
                    else
                    {
                        if (useIndexColor) lyr.pixlmap.DrawLineCircle(previousPoint, mousePosition, brushWidth, indexColor);
                        else lyr.pixlmap.DrawLineCircle(context, previousPoint, mousePosition, brushWidth, drawColor);
                    }

                    lyr.image = lyr.pixlmap.GetImage(context);
                    context.EditorControl.UpdateImageLayers = true;
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

                previewLayer.pixlmap.Clear();

                if (brushIsSquare)
                {
                    if (useIndexColor) previewLayer.pixlmap.DrawLineSquare(previousPoint, point, brushWidth, indexColor);
                    else previewLayer.pixlmap.DrawLineSquare(context, previousPoint, point, brushWidth, drawColor);
                }
                else
                {
                    if (useIndexColor) previewLayer.pixlmap.DrawLineCircle(previousPoint, point, brushWidth, indexColor);
                    else previewLayer.pixlmap.DrawLineCircle(context, previousPoint, point, brushWidth, drawColor);
                }

                previewLayer.image = previewLayer.pixlmap.GetImage(context);
            }
        }
    }
}
