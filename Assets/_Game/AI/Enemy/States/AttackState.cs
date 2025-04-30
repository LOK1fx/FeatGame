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
            Debug.Log("AttackState created with damage: " + damage);
        }

        public void Enter(AiAgent agent)
        {
            Debug.Log("AttackState Enter");
            agent.GetNavMeshAgent(out _navAgent);
            if (_navAgent != null)
            {
                _navAgent.isStopped = false;
                Debug.Log("NavAgent found and started");
            }
            else
            {
                Debug.LogError("NavAgent is null!");
            }
            _playerLayer = LayerMask.GetMask("Player");
            _currentCooldown = 0f;
            _currentAttackTime = 0f;
            _isAttacking = false;
            _hasDealtDamage = false;
        }

        public void Exit(AiAgent agent)
        {
            Debug.Log("AttackState Exit");
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
            {
                Debug.LogError("NavAgent is null in Update!");
                return;
            }

            var colliders = Physics.OverlapSphere(agent.transform.position, _attackRange * 2, _playerLayer);
            
            if (colliders.Length == 0)
            {
                Debug.Log("No players in range, switching to Idle");
                agent.StateMachine.SetState(AiStateId.Idle);
                return;
            }

            _currentTarget = colliders[0].GetComponent<Player>();
            if (_currentTarget == null)
            {
                Debug.LogError("Player component not found!");
                return;
            }

            float distanceToTarget = Vector3.Distance(agent.transform.position, _currentTarget.transform.position);
            Debug.Log($"Distance to target: {distanceToTarget}, Attack range: {_attackRange}");

            if (!_isAttacking)
            {
                if (_currentCooldown <= 0f)
                {
                    if (distanceToTarget <= _attackRange)
                    {
                        Debug.Log("Starting attack");
                        _isAttacking = true;
                        _currentAttackTime = 0f;
                        _navAgent.isStopped = true;
                    }
                    else
                    {
                        Debug.Log("Moving to target");
                        _navAgent.isStopped = false;
                        _navAgent.SetDestination(_currentTarget.transform.position);
                    }
                }
                else
                {
                    Debug.Log($"Cooldown: {_currentCooldown}");
                    _currentCooldown -= Time.deltaTime;
                    _navAgent.isStopped = false;
                    _navAgent.SetDestination(_currentTarget.transform.position);
                }
            }
            else
            {
                _currentAttackTime += Time.deltaTime;
                Debug.Log($"Attack progress: {_currentAttackTime}/{_attackDuration}");
                
                if (_currentAttackTime >= _attackDuration)
                {
                    Debug.Log("Attack finished, switching to Circle");
                    _isAttacking = false;
                    _currentCooldown = _attackCooldown;
                    _navAgent.isStopped = false;
                    agent.StateMachine.SetState(AiStateId.Circle);
                }
                else
                {
                    if (distanceToTarget <= _attackRange && !_hasDealtDamage)
                    {
                        Debug.Log("Dealing damage");
                        _currentTarget.TakeDamage(new Damage(_attackDamage, agent));
                        _hasDealtDamage = true;
                        agent.StateMachine.SetState(AiStateId.Circle);
                    }
                    else if (distanceToTarget > _attackRange)
                    {
                        Debug.Log("Target out of range, resuming chase");
                        _isAttacking = false;
                        _navAgent.isStopped = false;
                    }
                }
            }

            agent.transform.LookAt(_currentTarget.transform.position);
        }
    }
} 