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
    /// Interaction logic for ToolIcon.xaml
    /// </summary>
    public partial class ToolIcon : UserControl
    {
        readonly ToolBase Tool;
        readonly EditorContext Context;
        public ToolIcon(ToolBase tool, EditorContext context)
        {
            InitializeComponent();
            Tool = tool;
            Context = context;
            IconImg.Source = tool.Icon;
        }

        private void IconBtn_Click(object sender, RoutedEventArgs e)
        {
            if(Context.ActiveTool != Tool)
            {
                Context.SetActiveTool(Tool);
                Context.MainWindow.ToolOptions.Content = Tool.OptionsPanel;
            }
        }
    }
}
