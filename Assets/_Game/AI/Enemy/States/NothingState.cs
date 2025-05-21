using LOK1game.AI;
using UnityEngine.AI;

namespace LOK1game
{
    public class NothingState : IAiState
    {
        private NavMeshAgent _navAgent;

        public EAiStateId GetStateId() => EAiStateId.Nothing;

        public void Enter(AiAgent agent)
        {
            agent.GetNavMeshAgent(out _navAgent);

            if (_navAgent != null)
                _navAgent.isStopped = true;
        }

        public void Exit(AiAgent agent)
        {
            
        }

        public void OnGizmosLayer()
        {
            
        }

        public void Update(AiAgent agent)
        {
            
        }
    }
}