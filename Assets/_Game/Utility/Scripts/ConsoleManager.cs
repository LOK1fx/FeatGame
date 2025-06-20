using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace LOK1game.Utility
{
    public class ConsoleManager : MonoBehaviour
    {
        private Dictionary<string, MethodInfo> _commands = new Dictionary<string, MethodInfo>();
        private Dictionary<string, string> _commandDescriptions = new Dictionary<string, string>();
        private Dictionary<string, object> _commandInstances = new Dictionary<string, object>();
        private Console _console;

        private void Awake()
        {
            _console = GetComponent<Console>();
            if (_console == null)
            {
                Debug.LogError("Console компонент не найден на том же GameObject!");
                return;
            }

            RegisterCommands();
        }

        private void RegisterCommands()
        {
            // Находим все методы с атрибутом ConsoleCommand во всех сборках
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | 
                                                BindingFlags.Static | BindingFlags.Instance);
                    
                    foreach (var method in methods)
                    {
                        var commandAttr = method.GetCustomAttribute<ConsoleCommandAttribute>();
                        if (commandAttr != null)
                        {
                            var commandName = commandAttr.CommandName.ToLower();
                            _commands[commandName] = method;
                            _commandDescriptions[commandName] = commandAttr.Description;

                            // Если метод не статический, находим экземпляр объекта
                            if (!method.IsStatic)
                            {
                                var instance = FindObjectOfType(type);
                                if (instance != null)
                                {
                                    _commandInstances[commandName] = instance;
                                }
                                else
                                {
                                    LogWarning($"Не найден экземпляр {type.Name} для команды {commandName}");
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool ExecuteCommand(string commandLine)
        {
            if (string.IsNullOrEmpty(commandLine))
                return false;

            var parts = commandLine.Split(' ');
            var commandName = parts[0].ToLower();
            var args = parts.Skip(1).ToArray();

            if (_commands.TryGetValue(commandName, out var method))
            {
                try
                {
                    var parameters = method.GetParameters();
                    var convertedArgs = new object[parameters.Length];

                    // Преобразуем аргументы в нужные типы
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        if (i < args.Length)
                        {
                            convertedArgs[i] = Convert.ChangeType(args[i], parameters[i].ParameterType);
                        }
                        else if (parameters[i].HasDefaultValue)
                        {
                            convertedArgs[i] = parameters[i].DefaultValue;
                        }
                        else
                        {
                            LogError($"Недостаточно аргументов для команды {commandName}");
                            return false;
                        }
                    }

                    // Вызываем метод
                    object instance = null;
                    if (!method.IsStatic)
                    {
                        if (!_commandInstances.TryGetValue(commandName, out instance))
                        {
                            LogError($"Не найден экземпляр объекта для команды {commandName}");
                            return false;
                        }
                    }

                    method.Invoke(instance, convertedArgs);
                    return true;
                }
                catch (Exception e)
                {
                    LogError($"Ошибка при выполнении команды {commandName}: {e.Message}");
                    return false;
                }
            }

            LogError($"Команда {commandName} не найдена");
            return false;
        }

        public Dictionary<string, string> GetAllCommands()
        {
            return _commandDescriptions;
        }

        public void Log(string message)
        {
            _console.AddToOutput(message);
        }

        public void LogError(string message)
        {
            _console.AddToOutput($"<color=red>{message}</color>");
        }

        public void LogWarning(string message)
        {
            _console.AddToOutput($"<color=yellow>{message}</color>");
        }
    }
} 