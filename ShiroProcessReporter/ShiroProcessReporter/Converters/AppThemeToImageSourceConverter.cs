using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiroProcessReporter.Converters
{
    public class AppThemeToImageSourceConverter : IValueConverter
    {
        public ImageSource LightImage { get; set; }
        public ImageSource DarkImage { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (ApplicationTheme.Dark == value as ApplicationTheme?)
            {
                return DarkImage;
            }

            return LightImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
