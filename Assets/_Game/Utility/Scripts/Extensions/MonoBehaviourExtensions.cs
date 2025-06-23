using UnityEngine;

namespace LOK1game
{
    public static class MonoBehaviourExtensions
    {
        public static LOK1gameLogger GetLogger(this MonoBehaviour self, ELoggerGroup loggerGroup)
        {
            return App.Loggers.GetLogger(loggerGroup);
        }
    }
}
