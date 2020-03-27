using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace PixlSpriter
{
    public sealed class PanTool : ToolBase
    {
        public static PanTool Instance { get; private set; }

        public PanTool() {
            StreamResourceInfo info1 = Application.GetResourceStream(new Uri("res/cursors/grab_hand.cur", UriKind.Relative));
            StreamResourceInfo info2 = Application.GetResourceStream(new Uri("res/cursors/grab_hand_closed.cur", UriKind.Relative));
            grabHand = new Cursor(info1.Stream);
            grabHandClosed = new Cursor(info2.Stream);
            Instance = this;
            EditorContext.Toolbox = this;
        }

        public override string Name => "Pan Tool";

        public override string Description => "Hold and Drag to move the view.";

        private readonly ImageSource IconSource = Helper.GetResourceImage("res/tools/pantool.png");
        public override ImageSource Icon => IconSource;

        public override bool DisablePanning => true;

        int isPressed = 0;

        Point lastPos;

        public Cursor grabHand;
        public Cursor grabHandClosed;

        public override void MouseDown(EditorContext context, MouseButtonEventArgs args)
        {
            int ipl = isPressed;
            switch(args.ChangedButton)
            {
                case MouseButton.Left:
                    isPressed |= 1;
                    break;
                case MouseButton.Right:
                    isPressed |= 2;
                    break;
                case MouseButton.Middle:
                    isPressed |= 4;
                    break;
            }

            if(isPressed != ipl)
            {
                lastPos = args.GetPosition(context.EditorControl);
                Cursor = grabHandClosed;
            }
        }
        public override void StylusDown(EditorContext context, StylusDownEventArgs args)
        {
            isPressed |= 8;
            lastPos = args.GetPosition(context.EditorControl);
        }

        public override void MouseMove(EditorContext context, MouseEventArgs args)
        {
            if (isPressed > 0)
            {
                Point mousePos = args.GetPosition(context.EditorControl);
                Editor.PanEditor(context, Point.Subtract(mousePos, lastPos));
                lastPos = mousePos;
            }
        }

        public override void StylusMove(EditorContext context, StylusEventArgs args)
        {
            if(isPressed > 0)
            {
                if(!args.InAir)
                {
                    Point mousePos = args.GetPosition(context.EditorControl);
                    Editor.PanEditor(context, Point.Subtract(mousePos, lastPos));
                    lastPos = mousePos;
                }
            }
        }

        public override void MouseUp(EditorContext context, MouseButtonEventArgs args)
        {
            switch (args.ChangedButton)
            {
                case MouseButton.Left:
                    if ((isPressed & 1) == 1) isPressed -= 1;
                    break;
                case MouseButton.Right:
                    if ((isPressed & 2) == 2) isPressed -= 2;
                    break;
                case MouseButton.Middle:
                    if ((isPressed & 4) == 4) isPressed -= 4;
                    break;
            }

            if (isPressed > 0)
            {
                Cursor = grabHand;
            }
        }

        public override void StylusUp(EditorContext context, StylusEventArgs args)
        {
            if ((isPressed & 8) == 8) isPressed -= 8;
            if (isPressed > 0)
            {
                Cursor = grabHand;
            }
        }

        public override void KeyDown(EditorContext context, KeyEventArgs args)
        {
            // Removed - Space must be used in combination with left or right click to function.
            /* 
            if (args.Key == Key.Space)
            {
                isPressed |= 16;
                lastPos = Mouse.GetPosition(context.EditorControl);
            }
            */
        }

        public override void KeyUp(EditorContext context, KeyEventArgs args)
        {
            // Removed - Space must be used in combination with left or right click to function.
            /*
            if (args.Key == Key.Space)
            {
                if ((isPressed & 16) == 16)
                {
                    isPressed -= 16;
                    lastPos = Mouse.GetPosition(context.EditorControl);
                }
            }   
            */
        }
    }
}
