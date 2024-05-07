using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiroProcessReporter.Helper
{
    public static class Preferences
    {
        private static readonly Windows.Storage.ApplicationDataContainer LocalSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        public static T? Get<T>(string key) where T : struct
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            object value = LocalSettings.Values[key];

            if (value is null)
            {
                return null;
            }

            return (T)value;
        }

        public static T Get<T>(string key, T defaultValue)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return defaultValue;
            }

            object value = LocalSettings.Values[key];

            if (value is null)
            {
                return defaultValue;
            }

            return (T)value;
        }

        public static void Set<T>(string key, T value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            LocalSettings.Values[key] = value;
        }

    }
}
