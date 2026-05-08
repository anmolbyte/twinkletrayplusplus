using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TwinkleTrayAvalonia.Helpers
{
    public class TranslationHelper
    {
        private static Dictionary<string, string> _currentLanguage = new Dictionary<string, string>();
        private static Dictionary<string, string> _fallbackLanguage = new Dictionary<string, string>();

        public static void LoadLanguage(string langCode)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages", "localization", $"{langCode}.json");
                if (File.Exists(path))
                {
                    _currentLanguage = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path)) ?? new Dictionary<string, string>();
                }

                string fallbackPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages", "localization", "en.json");
                if (File.Exists(fallbackPath))
                {
                    _fallbackLanguage = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(fallbackPath)) ?? new Dictionary<string, string>();
                }
            }
            catch { }
        }

        public static string GetString(string key, params object[] args)
        {
            string format = "";
            if (_currentLanguage.TryGetValue(key, out string? value))
            {
                format = value;
            }
            else if (_fallbackLanguage.TryGetValue(key, out string? fallbackValue))
            {
                format = fallbackValue;
            }

            if (string.IsNullOrEmpty(format)) return key;

            for (int i = 0; i < args.Length; i++)
            {
                format = format.Replace($"{{{{{i + 1}}}}}", args[i]?.ToString());
            }

            return format;
        }
    }
}
