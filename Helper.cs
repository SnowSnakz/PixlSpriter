using System;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PixlSpriter
{
    public static class Helper
    {
        public static ImageSource GetResourceImage(string path)
        {
            Uri uri = new Uri("pack://application:,,,/" + Assembly.GetExecutingAssembly().FullName + ";component/" + path, UriKind.RelativeOrAbsolute);
            return new BitmapImage(uri);
        }
    }
}
