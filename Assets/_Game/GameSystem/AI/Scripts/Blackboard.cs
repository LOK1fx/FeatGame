using System;
using System.Collections.Generic;

namespace LOK1game.AI
{

    [Serializable]
    public class Blackboard
    {
        public List<Action> PassedActions { get; } = new();

        private Dictionary<string, BlackboardKey> _keyRegistry = new();
        private Dictionary<BlackboardKey, object> _entries = new();

        public void AddAction(Action action) => PassedActions.Add(action);
        public void ClearActions() => PassedActions.Clear();

        public bool TryGetValue<T>(BlackboardKey key, out T value)
        {
            if (_entries.TryGetValue(key, out var entry) && entry is BlackboardEntry<T> castedEntry)
            {
                value = castedEntry.Value;
                return true;
            }

            value = default;
            return false;
        }

        public void SetValue<T>(BlackboardKey key, T value)
        {
            _entries[key] = new BlackboardEntry<T>(key, value);
        }

        public BlackboardKey GetOrRegisterKey(string keyName)
        {
            if (_keyRegistry.TryGetValue(keyName, out BlackboardKey key) == false)
            {
                key = new BlackboardKey(keyName);
                _keyRegistry[keyName] = key;
            }

            return key;
        }

        public void Debug()
        {
            foreach(var entry in _entries)
            {
                var entryType = entry.Value.GetType();

                if (entryType.IsGenericType
                    && entryType.GetGenericTypeDefinition() == typeof(BlackboardEntry<>))
                {
                    var valueProperty = entryType.GetProperty("Value");

                    if (valueProperty == null)
                        continue;

                    var value = valueProperty.GetValue(entry.Value);

                    if (App.Loggers.TryGetLogger(ELoggerGroup.AI, out var logger))
                        logger.Push($"Key: {entry.Key}, Value: {value}");
                }
            }
        }

        public bool ContainsKey(BlackboardKey key) => _entries.ContainsKey(key);
        public void Remove(BlackboardKey key) => _entries.Remove(key);
    }
}
