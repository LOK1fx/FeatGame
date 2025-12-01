using LOK1game.AI;
using LOK1game.PlayerDomain;
using LOK1game.Tools;
using UnityEngine;

namespace LOK1game
{
    public class SandboxAIController : Controller<Player>
    {
        private readonly Arbiter _arbiter = new();
        private Blackboard _blackboard;
        private const string TargetPointKeyName = "TargetPoint";
        private const float MaxPitchAngle = 85f;
        private const float WaypointDirectionEpsilon = 0.001f;
        [SerializeField] private float _maxMouseInputDelta = 30f;

        protected override void Awake() { }

        protected override void OnPawnChanged(Player newPawn)
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
            if (ControlledPawn == null)
                return;

            InputContext.MovementInput = Vector2.zero;
            InputContext.LookInput = Vector2.zero;

            if (_blackboard != null)
            {
                foreach (var action in _arbiter.BlackboardIteration(_blackboard))
                {
                    action();
                }

                var waypointKey = _blackboard.GetOrRegisterKey(TargetPointKeyName);
                
                if (_blackboard.TryGetValue(waypointKey, out Vector3 waypoint))
                {
                    UpdateMovementInput(waypoint);
                    UpdateLookInput(waypoint);


                    if (ControlledPawn is Pawn pawn && Vector3.Distance(pawn.transform.position, waypoint) < 1.25f)
                    {
                        InputContext.FireFireButtonDown();
                        InputContext.FireJumpButtonDown();
                    }
                }
            }

            ControlledPawn?.OnInput(this, InputContext);
        }

        private void UpdateMovementInput(Vector3 waypoint)
        {
            if (ControlledPawn is not Player player)
                return;

            var pawn = (Pawn)player;
            var desiredDirection = waypoint - pawn.transform.position;
            desiredDirection.y = 0f;

            if (desiredDirection.sqrMagnitude <= WaypointDirectionEpsilon)
            {
                InputContext.MovementInput = Vector2.zero;
                return;
            }

            desiredDirection.Normalize();

            var cameraTransform = player.Camera?.GetCameraTransform();

            if (cameraTransform == null)
                return;

            var cameraForward = cameraTransform.forward;
            var cameraRight = cameraTransform.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;

            if (cameraForward.sqrMagnitude <= Mathf.Epsilon || cameraRight.sqrMagnitude <= Mathf.Epsilon)
                return;

            cameraForward.Normalize();
            cameraRight.Normalize();

            var x = Vector3.Dot(desiredDirection, cameraRight);
            var y = Vector3.Dot(desiredDirection, cameraForward);
            var localInput = new Vector2(x, y);

            if (localInput.sqrMagnitude > 1f)
                localInput.Normalize();

            InputContext.MovementInput = localInput;
        }

        private void UpdateLookInput(Vector3 waypoint)
        {
            if (ControlledPawn is not Player player || player.Camera == null)
                return;

            var cameraTransform = player.Camera.GetCameraTransform();

            if (cameraTransform == null)
                return;

            var toWaypoint = waypoint - cameraTransform.position;

            if (toWaypoint.sqrMagnitude <= WaypointDirectionEpsilon)
                return;

            var localDirection = cameraTransform.InverseTransformDirection(toWaypoint.normalized);

            var yawError = Mathf.Atan2(localDirection.x, localDirection.z) * Mathf.Rad2Deg;
            var pitchError = Mathf.Atan2(localDirection.y, localDirection.z) * Mathf.Rad2Deg;
            pitchError = Mathf.Clamp(pitchError, -MaxPitchAngle, MaxPitchAngle);

            var sensitivity = player.Camera.GetMouseInputScale();

            if (sensitivity <= Mathf.Epsilon)
                return;

            var yawDelta = Mathf.Clamp(yawError / sensitivity, -_maxMouseInputDelta, _maxMouseInputDelta);
            var pitchDelta = Mathf.Clamp(pitchError / sensitivity, -_maxMouseInputDelta, _maxMouseInputDelta);

            InputContext.LookInput = new Vector2(yawDelta, pitchDelta);
        }
    }
}
