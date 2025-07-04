using LOK1game.AI;
using LOK1game.PlayerDomain;
using UnityEngine;
using UnityEngine.AI;

namespace LOK1game
{
    public class IdleState : IAiState
    {
        private float _detectionRange = 15f;
        private NavMeshAgent _navAgent;
        private LayerMask _playerLayer;

        public void Enter(AiAgent agent)
        {
            agent.GetNavMeshAgent(out _navAgent);
            if (_navAgent != null)
            {
                _navAgent.isStopped = true;
            }
            _playerLayer = LayerMask.GetMask("Player");
        }

        public void Exit(AiAgent agent)
        {
            if (_navAgent != null)
            {
                _navAgent.isStopped = false;
            }
        }

        public EAiStateId GetStateId()
        {
            return EAiStateId.Idle;
        }

        public void OnGizmosLayer()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_navAgent.transform.position, _detectionRange);
        }

        public void Update(AiAgent agent)
        {
            if (_navAgent == null) return;

            var colliders = Physics.OverlapSphere(agent.transform.position, _detectionRange, _playerLayer);
            
            if (colliders.Length > 0)
                agent.StateMachine.SetState(EAiStateId.Chase);
        }
    }
} 