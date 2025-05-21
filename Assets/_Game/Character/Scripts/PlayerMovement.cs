using LOK1game.Game.Events;
using LOK1game.World;
using System;
using UnityEngine;

namespace LOK1game.PlayerDomain
{
    [RequireComponent(typeof(PlayerState), typeof(Rigidbody), typeof(CapsuleCollider))]
    public class PlayerMovement : MonoBehaviour
    {
        #region Events

        public event Action<float> OnLand;
        public event Action OnJump;

        public event Action OnStartCrouch;
        public event Action OnStopCrouch;
        public event Action OnStartSlide;

        public event Action OnStartSprint;
        public event Action OnStopSprint;

        public event Action OnMovementProcessed;

        #endregion

        public Vector3 ActualMoveDirection { get; private set; }
        public Rigidbody Rigidbody { get; private set; }
        public CapsuleCollider PlayerCollider { get; private set; }

        [SerializeField] private LayerMask _groundMask;

        [SerializeField] private float _crouchPlayerHeight = 1.5f;
        private float _defaultPlayerHeight;

        [SerializeField] private float _jumpCooldown = 0.1f;
        private float _currentJumpCooldown;

        [SerializeField] private int _maxJumps = 1;
        private int _jumpsLeft = 1;

        [SerializeField] private float _maxSlideTime;
        private float _currentSlideTime;

        [SerializeField] private float _minLandVelocity = -4f;

        public Transform DirectionTransform => _directionTransform;

        [SerializeField] private Transform _directionTransform;
        [SerializeField] private PlayerMovementParams _movementData;

        private PlayerState _playerState;

        private Vector3 _moveDirection;
        private RaycastHit _slopeHit;
        private Vector2 _iAxis = new Vector2(0f, 0f);

        private Vector3 _oldPosition;
        private Vector3 _previousVelocity;


        private void Awake()
        {
            _currentJumpCooldown = _jumpCooldown;

            Rigidbody = GetComponent<Rigidbody>();
            PlayerCollider = GetComponent<CapsuleCollider>();
            _playerState = GetComponent<PlayerState>();

            _defaultPlayerHeight = PlayerCollider.height;

            _oldPosition = transform.position;
        }

        private void Start()
        {
            if (GameWorld.TryGetWorld<DefaultGameWorld>(out var world))
            {
                if (world.DoubleJumpAllowrd)
                    AllowDoubleJump();
            }

            EventManager.AddListener<OnGameStateChangedEvent>(OnGameStateChanged);
        }

        private void Update()
        {
            UpdateCooldowns();

            if (_playerState.IsSliding && _playerState.OnGround)
            {
                _currentSlideTime += Time.deltaTime;
            }
            if (_currentSlideTime >= _maxSlideTime && _playerState.IsSliding)
            {
                Rigidbody.linearVelocity = Vector3.zero;

                StopCrouch();
                StartCrouch();

                _currentSlideTime = 0f;
            }

            ActualMoveDirection = (transform.position - _oldPosition).normalized;

            _oldPosition = transform.position;
        }

        private void FixedUpdate()
        {
            Move();

            _previousVelocity = Rigidbody.linearVelocity;
        }

        private void OnCollisionEnter(Collision collision)
        {
            var normal = collision.contacts[0].normal;

            if (CharacterMath.IsFloor(normal))
                _jumpsLeft = _maxJumps;

            if (_previousVelocity.y > _minLandVelocity)
                return;

            if (CharacterMath.IsFloor(normal))
                Land(_previousVelocity.y);
        }

        private void OnGameStateChanged(OnGameStateChangedEvent evt)
        {
            switch (evt.NewState)
            {
                case Game.EGameState.Paused:
                    SetAxisInput(Vector2.zero);
                    break;
            }
        }

        public void AllowDoubleJump()
        {
            _maxJumps = 2;
        }

        public void SetAxisInput(Vector2 input)
        {
            _iAxis = input;
        }

        private void Move()
        {
            if (!_playerState.InTransport)
            {
                Vector3 velocity;

                var moveParams = new CharacterMath.MoveParams(GetNonNormDirection(_iAxis), Rigidbody.linearVelocity);
                var slopeMoveParams = new CharacterMath.MoveParams(GetSlopeDirection(_iAxis), Rigidbody.linearVelocity);
                var slideMoveParams = new CharacterMath.MoveParams(Vector3.zero, Rigidbody.linearVelocity);

                if (_playerState.OnGround && !OnSlope() && !_playerState.IsSliding)
                {
                    velocity = MoveGround(moveParams, _playerState.IsSprinting, _playerState.IsCrouching);
                }
                else if (_playerState.OnGround && OnSlope() && !_playerState.IsSliding)
                {
                    velocity = MoveGround(slopeMoveParams, _playerState.IsSprinting, _playerState.IsCrouching);
                    Rigidbody.AddForce(GetSlopeDirection(_iAxis).normalized * 8f, ForceMode.Acceleration);
                }
                else if (_playerState.IsSliding)
                {
                    velocity = MoveAir(slideMoveParams);
                }
                else
                {
                    velocity = MoveAir(moveParams);
                }

                Rigidbody.linearVelocity = velocity;
            }

            OnMovementProcessed?.Invoke();
        }

        public void Jump()
        {
            if (CanJump())
            {
                var velocity = new Vector3(Rigidbody.linearVelocity.x, 0f, Rigidbody.linearVelocity.z);

                Rigidbody.linearVelocity = velocity;
                Rigidbody.AddForce(transform.up * _movementData.JumpForce, ForceMode.Impulse);

                ResetJumpCooldown();

                StopSprint();

                _jumpsLeft -= 1;

                OnJump?.Invoke();
            }
        }

        public void StartSprint()
        {
            OnStartSprint?.Invoke();
        }

        public void StopSprint()
        {
            OnStopSprint?.Invoke();
        }


        public void StartCrouch()
        {
            if (_playerState.IsWallruning) { return; }

            _currentSlideTime = 0f;

            PlayerCollider.center = Vector3.up * 0.75f;
            PlayerCollider.height = _crouchPlayerHeight;

            StopSprint();

            OnStartCrouch?.Invoke();
        }

        public void StartSlide()
        {
            _currentSlideTime = 0f;

            PlayerCollider.center = Vector3.up * 0.75f;
            PlayerCollider.height = _crouchPlayerHeight;

            var dir = GetDirection(_iAxis);

            StopSprint();

            Rigidbody.AddForce(dir, ForceMode.VelocityChange);

            OnStartSlide?.Invoke();
        }

        public void StopCrouch()
        {
            PlayerCollider.height = _defaultPlayerHeight;
            PlayerCollider.center = Vector3.up;

            OnStopCrouch?.Invoke();
        }

        public bool CanStand()
        {
            if (Physics.Raycast(transform.position, Vector3.up, out RaycastHit hit, 2f, _groundMask, QueryTriggerInteraction.Ignore))
                return false;

            return true;
        }

        public bool CanJump()
        {
            if (_currentJumpCooldown <= 0 && !_playerState.InTransport && _playerState.OnGround && _jumpsLeft > 0)
                return true;
            if (_jumpsLeft >= 1 && _maxJumps > 1 && _playerState.OnGround == false)
                return true;

            return false;
        }

        public void Land(float yVelocity)
        {
            var velocity = new Vector3(Rigidbody.linearVelocity.x, 0f, Rigidbody.linearVelocity.z);

            Rigidbody.linearVelocity = velocity;
            _jumpsLeft = _maxJumps;

            OnLand?.Invoke(yVelocity);
        }

        private void UpdateCooldowns()
        {
            if (_currentJumpCooldown > 0)
                _currentJumpCooldown -= Time.deltaTime;
        }

        public void ResetJumpCooldown()
        {
            _currentJumpCooldown = _jumpCooldown;
        }

        private bool OnSlope()
        {
            if (Physics.Raycast(transform.position + (Vector3.up * 0.2f), -transform.up, out _slopeHit, 0.3f, _groundMask))
            {
                if (_slopeHit.normal != Vector3.up)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public Vector3 GetSlopeDirection(Vector2 input)
        {
            return CharacterMath.Project(GetDirection(input), _slopeHit.normal);
        }

        /// <summary>
        /// ��������� �� �������������� ����������� ����� ������������ ������.
        /// �������� ��� ���������� � ��������
        /// </summary>
        /// <param name="input">����</param>
        /// <returns>Direction</returns>
        public Vector3 GetNonNormDirection(Vector2 input)
        {
            var direction = new Vector3(input.x, 0f, input.y);

            direction = DirectionTransform.TransformDirection(direction);

            _moveDirection = direction;

            return direction;
        }

        public Vector3 GetDirection(Vector2 input)
        {
            return GetNonNormDirection(input).normalized;
        }

        public Vector2 GetInputMoveAxis()
        {
            return _iAxis;
        }

        public int GetRoundedSpeed()
        {
            return Mathf.RoundToInt(GetSpeed());
        }

        public float GetSpeed()
        {
            return new Vector3(Rigidbody.linearVelocity.x, 0f, Rigidbody.linearVelocity.z).magnitude;
        }

        #region Acceleratation

        public Vector3 MoveGround(CharacterMath.MoveParams moveParams, bool sprint = false, bool crouch = false)
        {
            float t_speed = moveParams.PreviousVelocity.magnitude;

            if (t_speed != 0)
            {
                float drop = t_speed * _movementData.Friction * Time.fixedDeltaTime;
                moveParams.PreviousVelocity *= Mathf.Max(t_speed - drop, 0) / t_speed;
            }

            if (!sprint && !crouch)
            {
                return AccelerateVelocity(_movementData.WalkGroundAccelerate, _movementData.WalkGroundMaxVelocity, moveParams);
            }
            else if (!crouch)
            {
                return AccelerateVelocity(_movementData.SprintGoundAccelerate, _movementData.SprintGoundMaxVelocity, moveParams);
            }
            else
            {
                return AccelerateVelocity(_movementData.CrouchGroundAccelerate, _movementData.CrouchGroundMaxVelocity, moveParams);
            }
        }

        public Vector3 MoveAir(CharacterMath.MoveParams moveParams)
        {
            return AccelerateVelocity(_movementData.AirAccelerate, _movementData.AirMaxVelocity, moveParams);
        }

        private Vector3 AccelerateVelocity(float min, float max, CharacterMath.MoveParams moveParams)
        {
            return CharacterMath.Accelerate(moveParams, min, max, Time.fixedDeltaTime);
        }

        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(transform.position + Vector3.up, ActualMoveDirection);

            if (!OnSlope()) { return; }

            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, transform.position + _slopeHit.normal * 3f);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + CharacterMath.Project(_moveDirection, _slopeHit.normal) * 3f);
        }
    }
}