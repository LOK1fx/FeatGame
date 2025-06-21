using System;
using UnityEngine;

namespace LOK1game.Utility
{
    public class ExampleCommands : MonoBehaviour
    {
        private ConsoleManager _consoleManager;

        private void Awake()
        {
            _consoleManager = GetComponent<ConsoleManager>();
        }

        [ConsoleCommand("set_time_scale", "Sets timescale (0.0 - 1.0)")]
        public void SetTimeScale(float scale)
        {
            Time.timeScale = Mathf.Clamp01(scale);
            _consoleManager.Log($"Масштаб времени установлен на {Time.timeScale}");
        }

        [ConsoleCommand("print", "Print provided message into console")]
        public void PrintMessage(string message)
        {
            _consoleManager.Log($"Сообщение: {message}");
        }

        [ConsoleCommand("set_fps", "Set limits on FPS")]
        public void SetFPS(int fps)
        {
            Application.targetFrameRate = fps;
            _consoleManager.Log($"Ограничение FPS установлено на {fps}");
        }

        [ConsoleCommand("set_game_mode", "Changes current GameMode by GameModeId")]
        public void SetGameMode(int gameModeId)
        {
            try
            {
                App.ProjectContext.GameModeManager.SetGameMode((EGameModeId)Convert.ToUInt16(gameModeId));
            }
            catch (Exception ex)
            {
                _consoleManager.LogError(ex.ToString());
            }
        }

        [ConsoleCommand("r_graphics", "Sets the graphics level by index")]
        public void SetGraphicsLevel(int index)
        {
            QualitySettings.SetQualityLevel(index);
        }
    }
} 