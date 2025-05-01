using LOK1game.AI;
using LOK1game.PlayerDomain;
using UnityEngine;
using UnityEngine.AI;

namespace LOK1game
{
    public class AttackState : IAiState
    {
        private readonly int _attackDamage;
        private float _attackRange = 1.5f;
        private float _attackCooldown = 2f;
        private float _currentCooldown = 0f;
        private float _attackDuration = 1f;
        private float _currentAttackTime = 0f;
        private bool _isAttacking = false;
        private bool _hasDealtDamage = false;
        private NavMeshAgent _navAgent;
        private LayerMask _playerLayer;
        private Player _currentTarget;

        public AttackState(int damage)
        {
            _attackDamage = damage;
        }

        public void Enter(AiAgent agent)
        {
            agent.GetNavMeshAgent(out _navAgent);
            if (_navAgent != null)
            {
                _navAgent.isStopped = false;
                agent.GetAILogger().Push("NavAgent found and started");
            }
            else
            {
                agent.GetAILogger().PushError("NavAgent is null!");
            }
            _playerLayer = LayerMask.GetMask("Player");
            _currentCooldown = 0f;
            _currentAttackTime = 0f;
            _isAttacking = false;
            _hasDealtDamage = false;
        }

        public void Exit(AiAgent agent)
        {
            if (_navAgent != null)
            {
                _navAgent.isStopped = true;
            }
            _currentTarget = null;
            _hasDealtDamage = false;
        }

        public AiStateId GetStateId()
        {
            return AiStateId.Attack;
        }

        public void Update(AiAgent agent)
        {
            if (_navAgent == null)
                return;

            var colliders = Physics.OverlapSphere(agent.transform.position, _attackRange * 2, _playerLayer);
            
            if (colliders.Length == 0)
            {
                agent.StateMachine.SetState(AiStateId.Idle);
                return;
            }

            _currentTarget = colliders[0].GetComponent<Player>();
            if (_currentTarget == null)
                return;

            var distanceToTarget = Vector3.Distance(agent.transform.position, _currentTarget.transform.position);

            if (!_isAttacking)
            {
                if (_currentCooldown <= 0f)
                {
                    if (distanceToTarget <= _attackRange)
                    {
                        agent.GetAILogger().Push("Starting attack");
                        _isAttacking = true;
                        _currentAttackTime = 0f;
                        _navAgent.isStopped = true;
                    }
                    else
                    {
                        _navAgent.isStopped = false;
                        _navAgent.SetDestination(_currentTarget.transform.position);
                    }
                }
                else
                {
                    agent.GetAILogger().Push($"Cooldown: {_currentCooldown}");
                    _currentCooldown -= Time.deltaTime;
                    _navAgent.isStopped = false;
                    _navAgent.SetDestination(_currentTarget.transform.position);
                }
            }
            else
            {
                _currentAttackTime += Time.deltaTime;
                
                if (_currentAttackTime >= _attackDuration)
                {
                    _isAttacking = false;
                    _currentCooldown = _attackCooldown;
                    _navAgent.isStopped = false;
                    agent.StateMachine.SetState(AiStateId.Circle);
                }
                else
                {
                    if (distanceToTarget <= _attackRange && !_hasDealtDamage)
                    {
                        agent.GetAILogger().Push("Dealing damage");
                        _currentTarget.TakeDamage(new Damage(_attackDamage, agent));
                        _hasDealtDamage = true;
                        agent.StateMachine.SetState(AiStateId.Circle);
                    }
                    else if (distanceToTarget > _attackRange)
                    {
                        agent.GetAILogger().Push("Target out of range, resuming chase");
                        _isAttacking = false;
                        _navAgent.isStopped = false;
                    }
                }
            }

            if (_currentTarget != null)
                agent.transform.LookAt(_currentTarget.transform.position);
        }
    }
} 