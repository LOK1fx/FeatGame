using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LOK1game.PlayerDomain
{
    public enum EGroundCheckerShape
    {
        None = 0,
        Box,
        Sphere
    }

    public class PlayerState : MonoBehaviour
    {
        public bool OnGround { get; private set; }
        public bool IsSliding { get; private set; }
        public bool IsSprinting { get; private set; }
        public bool IsCrouching { get; private set; }
        public bool InTransport { get; private set; }
        public bool IsWallruning { get; private set; }

        [SerializeField] private LayerMask _groundMask;
        [SerializeField] private EGroundCheckerShape _groundCheckerShape;
        [SerializeField] private Vector3 _groundCheckerPosition;
        [SerializeField] private Vector3 _groundCheckerSize;
        [SerializeField] private float _movementThreshold = 4f;

        private PlayerMovement _playerMovement;

        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();

            _playerMovement.OnStartCrouch += () => IsCrouching = true;
            _playerMovement.OnStartSlide += () => IsSliding = true;
            _playerMovement.OnStopCrouch += OnStopCrouching;
            _playerMovement.OnStartSprint += OnStartSprintnig;
            _playerMovement.OnStopSprint += OnStopSprinting;
        }

        private void OnDestroy()
        {
            _playerMovement.OnStartCrouch -= () => IsCrouching = true;
            _playerMovement.OnStartSlide -= () => IsSliding = true;
            _playerMovement.OnStopCrouch -= OnStopCrouching;
            _playerMovement.OnStartSprint -= OnStartSprintnig;
            _playerMovement.OnStopSprint -= OnStopSprinting;
        }

        private void OnStopSprinting()
        {
            IsSprinting = false;
        }

        private void OnStartSprintnig()
        {
            IsSprinting = true;
        }

        private void FixedUpdate()
        {
            switch (_groundCheckerShape)
            {
                case EGroundCheckerShape.None:
                    break;
                case EGroundCheckerShape.Box:
                    CheckGroundBox();
                    break;
                case EGroundCheckerShape.Sphere:
                    CheckGroundSphere();
                    break;
                default:
                    CheckGroundSphere();
                    break;
            }
        }

        private void CheckGroundBox()
        {
            if (Physics.CheckBox(GetCheckerCenter(), _groundCheckerSize, Quaternion.identity, _groundMask, QueryTriggerInteraction.Ignore))
                OnGround = true;
            else
                OnGround = false;
        }

        private void CheckGroundSphere()
        {
            if (Physics.CheckSphere(GetCheckerCenter(), _groundCheckerSize.magnitude, _groundMask, QueryTriggerInteraction.Ignore))
                OnGround = true;
            else
                OnGround = false;
        }

        private Vector3 GetCheckerCenter()
        {
            return transform.position + _groundCheckerPosition;
        }

        public bool IsMoving()
        {
            if(_playerMovement.Rigidbody.linearVelocity.magnitude > _movementThreshold)
                return true;

            return false;
        }

        private void OnStopCrouching()
        {
            IsCrouching = false;
            IsSliding = false;
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;

            switch (_groundCheckerShape)
            {
                case EGroundCheckerShape.None:
                    break;
                case EGroundCheckerShape.Box:
                    DrawGizmoCheckerBox();
                    break;
                case EGroundCheckerShape.Sphere:
                    DrawGizmoCheckerSphere();
                    break;
                default:
                    DrawGizmoCheckerSphere();
                    break;
            }

            DrawForwardLines();
        }

        private void DrawGizmoCheckerBox()
        {
            Gizmos.DrawWireCube(GetCheckerCenter(), _groundCheckerSize * 2);
        }

        private void DrawGizmoCheckerSphere()
        {
            Gizmos.DrawWireSphere(GetCheckerCenter(), _groundCheckerSize.magnitude);
        }

        private void DrawForwardLines()
        {
            Gizmos.color = Color.blue;

            var lineLength = 1f;
            var startPos = transform.position + Vector3.forward * 0.125f;
            var angles = new float[]{ 0f, 15f, 30f, 45f };
            
            foreach (float angle in angles)
            {
                var direction = Quaternion.Euler(angle * -1f, 0f, 0) * transform.forward;
                var endPos = startPos + direction * lineLength;
                Gizmos.DrawLine(startPos, endPos);
                
                var textPos = endPos + Vector3.up * 0.05f;
                Handles.Label(textPos, $"{angle}°");
            }
        }
#endif
    }
}