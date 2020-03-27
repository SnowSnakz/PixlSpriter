using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PixlSpriter
{
    public abstract class ToolBase
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract ImageSource Icon { get; }

        public Cursor Cursor { get; protected set; }

        public virtual bool ReserveMiddleClick { get => false; }
        public virtual bool ReserveSpaceKey { get => false; }
        public virtual bool DisablePanning { get => false; }

        public Grid OptionsPanel { get; } = new Grid();

        public virtual void Activated(EditorContext context) { }
        public virtual void Deactivated(EditorContext context) { }
        public virtual void MouseDown(EditorContext context, MouseButtonEventArgs args) { }
        public virtual void MouseUp(EditorContext context, MouseButtonEventArgs args) { }
        public virtual void MouseMove(EditorContext context, MouseEventArgs args) { }
        public virtual void StylusDown(EditorContext context, StylusDownEventArgs args) { }
        public virtual void StylusMove(EditorContext context, StylusEventArgs args) { }
        public virtual void StylusUp(EditorContext context, StylusEventArgs args) { }
        public virtual void StylusButtonDown(EditorContext context, StylusButtonEventArgs args) { }
        public virtual void StylusButtonUp(EditorContext context, StylusButtonEventArgs args) { }
        public virtual void KeyDown(EditorContext context, KeyEventArgs args) { }
        public virtual void KeyUp(EditorContext context, KeyEventArgs args) { }

        protected static void SetCursor(ToolBase tool, Cursor cursor)
        {
            tool.Cursor = cursor;
        }


    }
}
