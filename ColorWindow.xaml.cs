using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace PixlSpriter
{
    /// <summary>
    /// Interaction logic for ColorWindow.xaml
    /// </summary>
    public partial class ColorWindow : MetroWindow
    {
        bool IsPrimary;
        EditorContext Context;

        public ColorWindow(bool isPrimary, EditorContext context)
        {
            InitializeComponent();
            Context = context;
            IsPrimary = isPrimary;
        }

        private void ColorCanvas_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (!e.NewValue.HasValue) return;
            if (IsPrimary)
            {
                Context.RPrimaryColor = e.NewValue.Value;
                Context.IPrimaryColor = -1;
            }
            else
            {
                Context.RSecondaryColor = e.NewValue.Value;
                Context.ISecondaryColor = -1;
            }
        }

        private void MetroWindow_Activated(object sender, EventArgs e)
        {
            Focus();
        }

        private void MetroWindow_LostFocus(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
