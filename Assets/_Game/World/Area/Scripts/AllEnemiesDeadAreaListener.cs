using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEditor;

namespace LOK1game
{
    [RequireComponent(typeof(Collider))]
    public class AllEnemiesDeadAreaListener : MonoBehaviour
    {
        public UnityEvent OnConditionCompleted;

        private Collider _collider;
        private readonly Dictionary<string, EnemyBase> _enemiesInArea = new();

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void Start()
        {
            var colliders = GetCollidersInBox();
            
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent<EnemyBase>(out var enemy))
                {
                    enemy.TryGetUniqueId(out var enemyId);

                    _enemiesInArea.Add(enemyId.ToString(), enemy);
                    enemy.OnDied += OnEnemyDied;
                }
            }
            
            if (_enemiesInArea.Count == 0)
                OnConditionCompleted?.Invoke();
        }

        private void OnDrawGizmosSelected()
        {
            if (_collider == null)
                _collider = GetComponent<Collider>();

            var colliders = GetCollidersInBox();

            foreach(var collider in colliders)
            {
                if (collider.TryGetComponent<EnemyBase>(out var enemy))
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(transform.position, enemy.transform.position);
                }
            }
        }

        private void OnEnemyDied(string enemyId)
        {
            _enemiesInArea.Remove(enemyId);

            if (_enemiesInArea.Count == 0)
                OnConditionCompleted?.Invoke();
        }

        private Collider[] GetCollidersInBox()
        {
            return Physics.OverlapBox(_collider.bounds.center, _collider.bounds.extents, transform.rotation);
        }

        private void OnDestroy()
        {
            foreach (var enemy in _enemiesInArea.Values)
            {
                if (enemy != null)
                    enemy.OnDied -= OnEnemyDied;
            }
        }
    }
}
