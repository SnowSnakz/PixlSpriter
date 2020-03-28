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
    public class EditorImageLayer
    {
        public bool PreviewLayer { get; set; }
        public Pixlmap pixlmap;
        public Guid ID { get; } = Guid.NewGuid();
        private ImageSource imgsrc;
        public ImageSource image 
        { 
            get => imgsrc; 
            set {
                if (control == null) control = new Image() { Stretch = Stretch.Fill };
                imgsrc = value;
                control.Source = image;
                control.InvalidateVisual();
            } 
        }
        public Image control;
    }
}
