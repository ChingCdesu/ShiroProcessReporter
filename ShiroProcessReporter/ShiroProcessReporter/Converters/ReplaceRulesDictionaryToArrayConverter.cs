using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using ShiroProcessReporter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiroProcessReporter.Converters
{
    public class ReplaceRulesDictionaryToArrayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not Dictionary<string, string>)
            {
                return new List<ReplaceRule>();
            }

            Dictionary<string, string> pairs = value as Dictionary<string, string>;
            return pairs!.Select(item => new ReplaceRule { Original = item.Key, Replacement = item.Value }).ToArray();
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
