using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace LOK1game
{
    public class ApplicationUpdateManager : MonoBehaviour
    {
        private static List<IApplicationUpdatable> _updatables = new();
        private static List<IApplicationUpdatable> _pendingUpdatables = new();

        private static int _currentIndex = 0;

        private void Update()
        {
            for (_currentIndex = _updatables.Count - 1; _currentIndex >= 0; _currentIndex--)
                _updatables[_currentIndex].ApplicationUpdate();

            _updatables.AddRange(_pendingUpdatables);
            _pendingUpdatables.Clear();
        }

        public static void Register(IApplicationUpdatable updatable)
        {
            _pendingUpdatables.Add(updatable);
        }

        public static void Unregister(IApplicationUpdatable updatable)
        {
            _updatables.Remove(updatable);
            _currentIndex--;
        }
    }
}
