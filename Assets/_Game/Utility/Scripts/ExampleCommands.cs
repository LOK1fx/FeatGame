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

        [ConsoleCommand("sv_timescale", "Sets timescale (0.0 - 1.0)")]
        private void SetTimeScale(float scale)
        {
            Time.timeScale = Mathf.Clamp01(scale);
            _consoleManager.Log($"Time.timeScale = {Time.timeScale}");
        }

        [ConsoleCommand("print", "Print provided message into console")]
        private void PrintMessage(string message)
        {
            _consoleManager.Log($"Message: {message}");
        }

        [ConsoleCommand("echo", "Echos into logs")]
        private void EchoMessage(string @string)
        {
            Debug.Log(@string);
        }

        [ConsoleCommand("r_set_fps", "Set limits on FPS")]
        private void SetFPS(int fps)
        {
            Application.targetFrameRate = fps;
            _consoleManager.Log($"Application.targetFrameRate = {fps}");
        }

        [ConsoleCommand("r_graphics", "Sets the graphics level by index")]
        private void SetGraphicsLevel(int index)
        {
            QualitySettings.SetQualityLevel(index);
            _consoleManager.Log($"Quality.Graphics = {QualitySettings.GetQualityLevel()}");
        }
    }
} 