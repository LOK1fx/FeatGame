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
        private List<string> _commandHistory = new();
        private int _currentHistoryIndex = -1;
        private bool _isVisible;

        private void Awake()
        {
            _consoleManager = GetComponent<ConsoleManager>();

            _inputField.onSubmit.AddListener(OnCommandSubmitted);
            _consolePanel.InstaHide();
        }

        private void Start()
        {
            Print("<color=grey><b>[Console]</b></color> Type <b>help</b> for commands list");
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
                var pos = _commandHistory.Count - 1 - _currentHistoryIndex;
                _inputField.text = _commandHistory[pos];
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
            {
                _inputField.ActivateInputField();
            }   
            else
            {
                _inputField.DeactivateInputField();
                _inputField.text = string.Empty;
            } 
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
            var commandInfos = _consoleManager.GetAllCommandInfos();
            var text = "";

            foreach (var info in commandInfos.OrderBy(c => c.Name))
            {
                var args = string.Join(" ", info.Parameters.Select(p => $"<{p.Name}: {GetTypeAlias(p.ParameterType)}>{(p.IsOptional ? $"={p.DefaultValue}" : "")}"));
                text += $"<b>{info.Name}</b> <color=grey>{args}: {info.Description}</color>\n";
            }

            AddToOutput(text);
        }

        private string GetTypeAlias(System.Type type)
        {
            if (type == typeof(int)) return "int";
            if (type == typeof(float)) return "float";
            if (type == typeof(double)) return "double";
            if (type == typeof(long)) return "long";
            if (type == typeof(uint)) return "uint";
            if (type == typeof(ulong)) return "ulong";
            if (type == typeof(short)) return "short";
            if (type == typeof(ushort)) return "ushort";
            if (type == typeof(byte)) return "byte";
            if (type == typeof(sbyte)) return "sbyte";
            if (type == typeof(char)) return "char";
            if (type == typeof(decimal)) return "decimal";
            if (type == typeof(string)) return "string";
            if (type == typeof(bool)) return "bool";
            if (type == typeof(Vector2)) return "Vector2";
            if (type == typeof(Vector3)) return "Vector3";
            if (type == typeof(Color)) return "Color";

            return type.Name;
        }

        public void AddToOutput(string text)
        {
            _outputText.text += text + "\n";

            var lines = _outputText.text.Split('\n');
            if (lines.Length > _maxOutputLines)
                _outputText.text = string.Join("\n", lines.Skip(lines.Length - _maxOutputLines));

            UpdateContentSize();
            ScrollToBottom();
        }

        private void UpdateContentSize()
        {
            if (_scrollRect.content != null && _outputText != null)
            {
                var totalHeight = _outputText.preferredHeight + _outputText.fontSize;
                var contentRect = _scrollRect.content;
                contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, totalHeight);
            }
        }

        private void ScrollToBottom()
        {
            _scrollRect.verticalNormalizedPosition = 0f;
            _scrollRect.horizontalNormalizedPosition = 0f;
        }

        [ConsoleCommand("clear", "Clear console")]
        public void ClearConsole()
        {
            _outputText.text = string.Empty;
            ScrollToBottom();
        }

        public static void Print(string message)
        {
            App.DevConsole.Log(message);
        }
    }
} 