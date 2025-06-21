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
            _consoleManager.Log($"Time.timeScale = {Time.timeScale}");
        }

        [ConsoleCommand("print", "Print provided message into console")]
        public void PrintMessage(string message)
        {
            _consoleManager.Log($"Message: {message}");
        }

        [ConsoleCommand("set_fps", "Set limits on FPS")]
        public void SetFPS(int fps)
        {
            Application.targetFrameRate = fps;
            _consoleManager.Log($"Application.targetFrameRate = {fps}");
        }

        [ConsoleCommand("r_graphics", "Sets the graphics level by index")]
        public void SetGraphicsLevel(int index)
        {
            QualitySettings.SetQualityLevel(index);
            _consoleManager.Log($"Quality.Graphics = {QualitySettings.GetQualityLevel()}");
        }
    }
} 