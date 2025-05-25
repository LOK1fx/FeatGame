using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace LOK1game
{
    public static class Settings
    {
        public static event Action<float> OnSensivityChanged;

        private const string PLAYER_SETTING_PREFIX = "PlayerSettings_";
        private const string SENSITIVITY = PLAYER_SETTING_PREFIX + "Sensitivity";

        public static bool TryGetSensivity(out float sensitivity)
        {
            var sens = PlayerPrefs.GetFloat(SENSITIVITY);

            if (sens == 0)
            {
                sensitivity = 15f; // default value
                return false;
            }
            else
            {
                sensitivity = sens;
                return true;
            }
        }

        public static void SetSensitivity(float value)
        {
            PlayerPrefs.SetFloat(SENSITIVITY, value);
            OnSensivityChanged?.Invoke(value);
        }
    }
}