using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace LOK1game.Utility
{
    [RequireComponent(typeof(Console))]
    public class ConsoleManager : MonoBehaviour
    {
        private readonly Dictionary<string, MethodInfo> _commands = new Dictionary<string, MethodInfo>();
        private readonly Dictionary<string, string> _commandDescriptions = new Dictionary<string, string>();
        private readonly Dictionary<string, object> _commandInstances = new Dictionary<string, object>();
        private Console _console;

        private void Awake()
        {
            _console = GetComponent<Console>();

            Application.logMessageReceived += OnUnityMessageReceived;

            RegisterCommands();
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= OnUnityMessageReceived;
        }

        private void OnUnityMessageReceived(string logMessage, string stackTrace, LogType logType)
        {
            switch (logType)
            {
                case LogType.Error:
                    LogError(logMessage);
                    break;
                case LogType.Assert:
                    Log(logMessage);
                    break;
                case LogType.Warning:
                    break;
                case LogType.Log:
                    Log(logMessage);
                    break;
                case LogType.Exception:
                    LogError(logMessage);
                    break;
                default:
                    break;
            }
        }

        private void RegisterCommands()
        {
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

                            if (!method.IsStatic)
                            {
                                var instance = FindFirstObjectByType(type);
                                if (instance != null)
                                {
                                    _commandInstances[commandName] = instance;
                                }
                                else
                                {
                                    LogWarning($"{type.Name} not found for command {commandName}");
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
                            LogError($"Not enoughn args for {commandName}");
                            return false;
                        }
                    }

                    object instance = null;
                    if (!method.IsStatic)
                    {
                        if (!_commandInstances.TryGetValue(commandName, out instance))
                        {
                            LogError($"Object not found for {commandName}");
                            return false;
                        }
                    }

                    method.Invoke(instance, convertedArgs);
                    return true;
                }
                catch (Exception e)
                {
                    LogError($"Error running {commandName}: {e.Message}");
                    return false;
                }
            }

            LogError($"Command {commandName} not found");
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