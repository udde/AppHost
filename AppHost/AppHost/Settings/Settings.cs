using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace AppHost.Settings
{
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        private static string defaultString = string.Empty;
        private static string defaultStringValue = "Default string value, key not found";
        
        #region Store the keys 
        //storing the added keys in a explicitly as the API has no GetAllKeys or similar
        //the keys are stored in a string sepatared by ',' as the add/get functions did not suport sets/lists
        private static string keyToStoredKeys = "key_to_stored_keys";
        private static string StoredKeys
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(keyToStoredKeys, defaultString);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(keyToStoredKeys, value);
            }
        }
        public static void StoreKey(string key)
        {
            string allKeys = AppSettings.GetValueOrDefault<string>(keyToStoredKeys, defaultString);

            if (allKeys.Split(',').Contains(key) == false)
                allKeys += $",{key}";

            AppSettings.AddOrUpdateValue<string>(keyToStoredKeys, allKeys);
        }
        #endregion

        #region Add and Get settings
        public static void AddSetting(string key, string value)
        {
            StoreKey(key);
            AppSettings.AddOrUpdateValue<string>(key, value);
        }

        public static string GetSetting(string key)
        {
            return AppSettings.GetValueOrDefault<string>(key, defaultStringValue);
        }
        #endregion

        public static string[] GetAllKeys()
        {
            return StoredKeys.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static Dictionary<string, string> GetAllSettings()
        {
            Dictionary<string, string> allSettings = new Dictionary<string, string>();

            foreach (string key in GetAllKeys())
            {
                allSettings.Add(key, AppSettings.GetValueOrDefault<string>(key, defaultStringValue));
            }

            return allSettings;
        }        
    }
}
