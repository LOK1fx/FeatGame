using LOK1game.AI;
using LOK1game.PlayerDomain;
using UnityEngine;
using UnityEngine.AI;

namespace LOK1game
{
    public class ChaseState : IAiState
    {
        private float _chaseRange = 10f;
        private float _circleRange = 4f;
        private NavMeshAgent _navAgent;
        private LayerMask _playerLayer;
        private Player _currentTarget;

        public void Enter(AiAgent agent)
        {
            agent.GetNavMeshAgent(out _navAgent);
            if (_navAgent != null)
            {
                _navAgent.isStopped = false;
            }
            _playerLayer = LayerMask.GetMask("Player");
        }

        public void Exit(AiAgent agent)
        {
            if (_navAgent != null)
            {
                _navAgent.isStopped = true;
            }
            _currentTarget = null;
        }

        public AiStateId GetStateId()
        {
            return AiStateId.Chase;
        }

        public void Update(AiAgent agent)
        {
            if (_navAgent == null) return;

            Collider[] colliders = Physics.OverlapSphere(agent.transform.position, _chaseRange, _playerLayer);
            
            if (colliders.Length == 0)
            {
                agent.StateMachine.SetState(AiStateId.Idle);
                return;
            }

            _currentTarget = colliders[0].GetComponent<Player>();
            if (_currentTarget == null) return;

            float distanceToPlayer = Vector3.Distance(agent.transform.position, _currentTarget.transform.position);

            if (distanceToPlayer <= _circleRange)
            {
                agent.StateMachine.SetState(AiStateId.Circle);
                return;
            }

            _navAgent.SetDestination(_currentTarget.transform.position);
        }
    }
}
