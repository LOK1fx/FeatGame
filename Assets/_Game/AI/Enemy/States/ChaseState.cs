using LOK1game.AI;
using LOK1game.PlayerDomain;
using UnityEngine;

namespace LOK1game
{
    public class ChaseState : IAiState
    {
        private Player _player;

        public void Enter(AiAgent agent)
        {
            _player = GameObject.FindFirstObjectByType<Player>();
        }

        public void Exit(AiAgent agent)
        {
            
        }

        public AiStateId GetStateId()
        {
            return AiStateId.Chase;
        }

        public void Update(AiAgent agent)
        {
            
        }
    }
}
