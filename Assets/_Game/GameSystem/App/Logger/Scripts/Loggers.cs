using System.Collections.Generic;
using UnityEngine;

namespace LOK1game.Utils
{
    [System.Serializable]
    public class LoggerContainer
    {
        public ELoggerGroup Group;
        public Color Color;
        public bool IsActivated;
    }

    [System.Serializable]
    public class Loggers
    {
        public Dictionary<ELoggerGroup, LOK1gameLogger> Value { get; private set; } = new Dictionary<ELoggerGroup, LOK1gameLogger>();

        public Loggers(Dictionary<ELoggerGroup, LOK1gameLogger> loggers)
        {
            Value = loggers;
        }

        public Loggers(LoggerContainer[] containers)
        {
            SwapLoggers(containers);
        }

        public void SwapLoggers(LoggerContainer[] containers)
        {
            Value.Clear();

            foreach (var container in containers)
            {
                Value.Add(container.Group, new LOK1gameLogger(container.Group, container.IsActivated, container.Color));
            }
        }

        public void SwapLoggers(Dictionary<ELoggerGroup, LOK1gameLogger> newloggers)
        {
            Value = newloggers;
        }

        public bool TryGetLogger(ELoggerGroup group, out LOK1gameLogger logger)
        {
            if (Value.ContainsKey(group))
            {
                logger = GetLogger(group);

                return true;
            }

            logger = null;

            return false;
        }

        public LOK1gameLogger GetLogger(ELoggerGroup group)
        {
            return Value[group];
        }
    }
}
