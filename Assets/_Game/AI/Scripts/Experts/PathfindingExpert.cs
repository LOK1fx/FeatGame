using LOK1game.PlayerDomain;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace LOK1game.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class PathfindingExpert : MonoBehaviour, IExpert
    {
        private NavMeshAgent _agent;
        private Blackboard _blackboard;
        private BlackboardKey _targetPointKey;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.updatePosition = false;
            _agent.updateRotation = false;
            _agent.updateUpAxis = false;
        }

        public void ConstructExpert(Blackboard blackboard)
        {
            _blackboard = blackboard;
            
            _targetPointKey = _blackboard.GetOrRegisterKey("TargetPoint");
            _blackboard.SetValue(_targetPointKey, Vector3.zero);

            StartCoroutine(FindPlayerRoutine());

            _blackboard.Debug();
        }

        public void Execute(Blackboard blackboard)
        {
            blackboard.AddAction(() =>
            {
                if (blackboard.TryGetValue(_targetPointKey, out Vector3 waypoint))
                {
                    blackboard.SetValue(_targetPointKey, _agent.pathEndPosition);
                }
            });
        }

        public int GetInsistence(Blackboard blackboard)
        {
            return 100;
        }

        private IEnumerator FindPlayerRoutine()
        {
            while (true)
            {
                var player = FindObjectsByType<Player>(FindObjectsSortMode.InstanceID)
                    .Where(p => p.IsLocallyControlled)
                    .FirstOrDefault();

                SetDestination(player ? player.transform.position : Vector3.zero);

                yield return new WaitForSeconds(1f);
            }
        }

        private void SetDestination(Vector3 point)
        {
            _agent.nextPosition = transform.position;
            _agent.destination = point;

            _blackboard.TryGetValue(_targetPointKey, out Vector3 boardpoint);

            App.Loggers.GetLogger(ELoggerGroup.AI).Push($"Next point is {point}");
            App.Loggers.GetLogger(ELoggerGroup.BaseInfo).Push($"Blackboard point is {boardpoint}");
        }
    }
}
