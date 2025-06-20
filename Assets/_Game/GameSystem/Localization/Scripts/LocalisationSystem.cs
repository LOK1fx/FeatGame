using System.Collections.Generic;
using UnityEngine;

namespace LOK1game
{
    public class LocalisationSystem
    {
        public enum ELanguage
        {
            English,
            Russian,
            Chinese
        }

        public static ELanguage Language 
        { 
            get => _currentLanguage;
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    if (IsInit)
                    {
                        UpdateCurrentDictionary();
                    }
                }
            }
        }

        private static ELanguage _currentLanguage = ELanguage.Russian;
        private static Dictionary<string, string> _currentDictionary = new Dictionary<string, string>();

        private static Dictionary<string, string> LocalisedEN;
        private static Dictionary<string, string> LocalisedRU;
        private static Dictionary<string, string> LocalisedZH;

        public static bool IsInit { get; private set; }

        public static void Init()
        {
            if (IsInit) return;

            try
            {
                var csvLoader = new CSVLoader();
                csvLoader.LoadCSV();

                LocalisedEN = csvLoader.GetDictionaryValues("en");
                LocalisedRU = csvLoader.GetDictionaryValues("ru");
                LocalisedZH = csvLoader.GetDictionaryValues("zh");

                if (LocalisedEN == null || LocalisedRU == null || LocalisedZH == null)
                {
                    Debug.LogError("Failed to load localization data!");
                    return;
                }

                IsInit = true;
                UpdateCurrentDictionary();
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error initializing localization system: {e.Message}");
            }
        }

        private static void UpdateCurrentDictionary()
        {
            if (!IsInit) return;

            var newDictionary = Language switch
            {
                ELanguage.English => LocalisedEN,
                ELanguage.Russian => LocalisedRU,
                ELanguage.Chinese => LocalisedZH,
                _ => LocalisedEN
            };

            if (newDictionary != null)
            {
                _currentDictionary = newDictionary;
            }
            else
            {
                Debug.LogError($"Failed to update current dictionary for language: {Language}");
            }
        }

        public static string GetLocalisedValue(string key)
        {
            if (!IsInit)
            {
                Debug.LogWarning("Localization system not initialized. Using key as fallback.");
                return key;
            }

            if (string.IsNullOrEmpty(key))
                return string.Empty;

            if (_currentDictionary == null)
            {
                Debug.LogError("Current dictionary is null! Attempting to reinitialize...");
                Init();
                if (_currentDictionary == null)
                {
                    return key;
                }
            }

            return _currentDictionary.TryGetValue(key, out var value) ? value : key;
        }
    }
}