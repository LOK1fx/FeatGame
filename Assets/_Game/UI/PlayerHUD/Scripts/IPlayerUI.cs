using LOK1game.PlayerDomain;
using UnityEngine;

namespace LOK1game.UI
{
    public interface IPlayerUI
    {

        void Bind(PlayerController controller, Player player);
    }
}
