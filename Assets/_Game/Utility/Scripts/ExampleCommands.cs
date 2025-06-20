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

        [ConsoleCommand("set_time_scale", "Устанавливает масштаб времени (0.0 - 1.0)")]
        public void SetTimeScale(float scale)
        {
            Time.timeScale = Mathf.Clamp01(scale);
            _consoleManager.Log($"Масштаб времени установлен на {Time.timeScale}");
        }

        [ConsoleCommand("print", "Выводит сообщение в консоль")]
        public void PrintMessage(string message)
        {
            _consoleManager.Log($"Сообщение: {message}");
        }

        [ConsoleCommand("set_fps", "Устанавливает ограничение FPS")]
        public void SetFPS(int fps)
        {
            Application.targetFrameRate = fps;
            _consoleManager.Log($"Ограничение FPS установлено на {fps}");
        }

        [ConsoleCommand("clear", "Очищает консоль")]
        public void ClearConsole()
        {
            _consoleManager.Log("Консоль очищена");
        }
    }
} 