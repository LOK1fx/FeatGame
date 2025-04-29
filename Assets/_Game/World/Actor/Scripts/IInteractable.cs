using LOK1game.PlayerDomain;
using UnityEngine;

namespace LOK1game
{
    public interface IInteractable
    {
        void OnInteract(Player sender);
    }
}
