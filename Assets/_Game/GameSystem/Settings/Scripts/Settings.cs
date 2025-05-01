using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LOK1game.Tools;

namespace LOK1game
{
    public static class Settings
    {
        private const string PLAYER_SETTING_PREFIX = "PlayerSettings_";
        private const string SENSITIVITY = PLAYER_SETTING_PREFIX + "Sensitivity";

        public static bool TryGetSensivity(out float sensitivity)
        {
            var sens = PlayerPrefs.GetFloat(SENSITIVITY);

            if (sens == 0)
            {
                sensitivity = 5f; // default value
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
        }
    }
}