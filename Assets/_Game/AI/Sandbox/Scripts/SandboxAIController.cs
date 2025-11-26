using Codice.Client.Common.FsNodeReaders;
using LOK1game.AI;
using LOK1game.PlayerDomain;
using LOK1game.Tools;
using UnityEngine;

namespace LOK1game
{
    public class SandboxAIController : Controller
    {
        private readonly Arbiter _arbiter = new();
        private Blackboard _blackboard;

        protected override void Awake() { }

        protected override void OnPawnChanged(IPawn newPawn)
        {
            if (newPawn == null)
            {
                _blackboard = null;
                _arbiter.ClearExperts();
            }
                

            if (newPawn is Player player)
            {
                _blackboard = player.Blackboard;

                var experts = player.GetComponents<IExpert>();

                foreach (var expert in experts)
                {
                    expert.ConstructExpert(_blackboard);
                    _arbiter.RegisterExpert(expert);
                }
            } 
        }

        public override void ApplicationUpdate()
        {
            if (_blackboard != null)
            {
                foreach (var action in _arbiter.BlackboardIteration(_blackboard))
                {
                    action();
                }

                var waypointKey = _blackboard.GetOrRegisterKey("TargetPoint");
                
                if (_blackboard.TryGetValue(waypointKey, out Vector3 waypoint))
                {
                    var point = ((Pawn)ControlledPawn).transform.position.GetDirectionTo(waypoint).normalized;
                    InputContext.MovementInput = new Vector2(point.x, point.z);
                }
            }

            InputContext.LookInput = new Vector2(0f, Mathf.Cos(Time.time) * 5f);

            ControlledPawn?.OnInput(this, InputContext);
        }
    }
}
