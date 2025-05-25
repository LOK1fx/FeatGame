using UnityEngine;

namespace LOK1game.Tools
{
    public static class DebugUtility
    {
        public static void AssertNotNull(Object @object)
        {
            Debug.Assert(@object != null, "Shouldn't be null.");
        }
    }
}
