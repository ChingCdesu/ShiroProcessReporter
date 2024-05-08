using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Management.Core;

namespace ShiroProcessReporter.Helper
{
    public static class Preferences
    {
        private static Windows.Storage.ApplicationDataContainer? LocalSettings;

        private static void Initialize()
        {
            LocalSettings = ApplicationDataManager.CreateForPackageFamily(Package.Current.Id.FamilyName).LocalSettings;
        }

        public static T? Get<T>(string key) where T : struct
        {
            if (LocalSettings is null)
            {
                Initialize();
            }

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
            if (LocalSettings is null)
            {
                Initialize();
            }

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
            if (LocalSettings is null)
            {
                Initialize();
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                return;
            }

            LocalSettings.Values[key] = value;
        }

    }
}
