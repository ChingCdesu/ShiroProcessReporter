using Microsoft.Extensions.Logging;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace ShiroProcessReporter.Converters
{
    public class LogLevelToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var brush = new SolidColorBrush(Colors.Gray); // 默认颜色
            if (Application.Current.Resources.TryGetValue("SystemFillColorCriticalBrush", out var errorColor) && value is LogLevel logLevel)
            {
                switch (logLevel)
                {
                    case LogLevel.Trace:
                    case LogLevel.Debug:
                        break;
                    case LogLevel.Information:
                        Application.Current.Resources.TryGetValue("SystemFillColorSuccessBrush", out var infoColor);
                        brush = (SolidColorBrush)infoColor;
                        break;
                    case LogLevel.Warning:
                        Application.Current.Resources.TryGetValue("SystemFillColorCautionBrush", out var warningColor);
                        brush = (SolidColorBrush)warningColor;
                        break;
                    case LogLevel.Error:
                        brush = (SolidColorBrush)errorColor;
                        break;
                }
            }
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
