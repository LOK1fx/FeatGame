using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using LOK1game.UI;
using LOK1game.Game.Events;

namespace LOK1game.Utility
{
    public class Console : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private TextMeshProUGUI _outputText;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private CanvasGroupFade _consolePanel;

        [Header("Settings")]
        [SerializeField] private int _maxOutputLines = 100;
        [SerializeField] private KeyCode _toggleKey = KeyCode.BackQuote;

        private ConsoleManager _consoleManager;
        private List<string> _commandHistory = new List<string>();
        private int _currentHistoryIndex = -1;
        private bool _isVisible;

        private void Awake()
        {
            _consoleManager = GetComponent<ConsoleManager>();
            if (_consoleManager == null)
            {
                Debug.LogError("ConsoleManager не найден на том же GameObject!");
                return;
            }

            _inputField.onSubmit.AddListener(OnCommandSubmitted);
            _consolePanel.InstaHide();
        }

        private void Update()
        {
            if (Input.GetKeyDown(_toggleKey))
                ToggleConsole();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isVisible)
                    ToggleConsole();
            }

            if (_isVisible)
                HandleHistoryNavigation();
        }

        private void HandleHistoryNavigation()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                NavigateHistory(-1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                NavigateHistory(1);
            }
        }

        private void NavigateHistory(int direction)
        {
            if (_commandHistory.Count == 0) return;

            _currentHistoryIndex = Mathf.Clamp(_currentHistoryIndex + direction, -1, _commandHistory.Count - 1);
            
            if (_currentHistoryIndex == -1)
            {
                _inputField.text = "";
            }
            else
            {
                _inputField.text = _commandHistory[_currentHistoryIndex];
                _inputField.caretPosition = _inputField.text.Length;
            }
        }

        private void ToggleConsole()
        {
            _isVisible = !_isVisible;
            _consolePanel.SetVisible(_isVisible);

            var evt = new OnDevConsoleStateChangedEvent(_isVisible);
            EventManager.Broadcast(evt);

            if (_isVisible)
                _inputField.ActivateInputField();
            else
                _inputField.DeactivateInputField();
        }

        private void OnCommandSubmitted(string command)
        {
            if (string.IsNullOrEmpty(command)) return;

            AddToOutput($"> {command}");

            if (command.ToLower() == "help")
                ShowHelp();
            else
                _consoleManager.ExecuteCommand(command);

            _commandHistory.Add(command);
            _currentHistoryIndex = -1;
            _inputField.text = "";
            _inputField.ActivateInputField();
        }

        private void ShowHelp()
        {
            var commands = _consoleManager.GetAllCommands();
            var helpText = "Доступные команды:\n";

            foreach (var command in commands.OrderBy(c => c.Key))
            {
                helpText += $"{command.Key}: {command.Value}\n";
            }

            AddToOutput(helpText);
        }

        public void AddToOutput(string text)
        {
            _outputText.text += text + "\n";

            var lines = _outputText.text.Split('\n');
            if (lines.Length > _maxOutputLines)
                _outputText.text = string.Join("\n", lines.Skip(lines.Length - _maxOutputLines));

            ResetPosition();
        }

        private void ResetPosition()
        {
            _scrollRect.verticalNormalizedPosition = 1f;
            _scrollRect.horizontalNormalizedPosition = 0f;
        }

        [ConsoleCommand("clear", "Clear console")]
        public void ClearConsole()
        {
            _outputText.text = string.Empty;
            ResetPosition();
        }

        public static void Print(string message)
        {
            App.DevConsole.Log(message);
        }
    }
} 