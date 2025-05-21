using LOK1game.AI;
using LOK1game.PlayerDomain;
using UnityEngine;
using UnityEngine.AI;

namespace LOK1game
{
    public class CircleState : IAiState
    {
        private float _circleRadius = 3f;
        private float _circleSpeed = 2f;
        private float _angle = 0f;
        private float _circleTime = 2f;
        private float _currentCircleTime = 0f;
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
            _currentCircleTime = 0f;
        }

        public void Exit(AiAgent agent)
        {
            if (_navAgent != null)
            {
                _navAgent.isStopped = true;
            }
            _currentTarget = null;
        }

        public EAiStateId GetStateId()
        {
            return EAiStateId.Circle;
        }

        public void OnGizmosLayer()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_navAgent.transform.position, _circleRadius);
        }

        public void Update(AiAgent agent)
        {
            if (_navAgent == null) return;

            Collider[] colliders = Physics.OverlapSphere(agent.transform.position, _circleRadius * 2, _playerLayer);
            
            if (colliders.Length == 0)
            {
                agent.StateMachine.SetState(EAiStateId.Idle);
                return;
            }

            _currentTarget = colliders[0].GetComponent<Player>();
            if (_currentTarget == null) return;

            _angle += _circleSpeed * Time.deltaTime;
            Vector3 circlePosition = _currentTarget.transform.position + 
                new Vector3(Mathf.Cos(_angle) * _circleRadius, 0, Mathf.Sin(_angle) * _circleRadius);
            
            _navAgent.SetDestination(circlePosition);
            agent.transform.LookAt(_currentTarget.transform.position);

            _currentCircleTime += Time.deltaTime;
            if (_currentCircleTime >= _circleTime)
            {
                agent.StateMachine.SetState(EAiStateId.Attack);
            }
        }
    }
} 