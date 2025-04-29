using LOK1game.PlayerDomain;
using UnityEngine;
using UnityEngine.Events;

namespace LOK1game
{
    public class ScriptableIntrractable : MonoBehaviour, IInteractable
    {
        public UnityEvent OnInteracted;

        public void OnInteract(Player sender)
        {
            OnInteracted?.Invoke();
        }
    }
}
