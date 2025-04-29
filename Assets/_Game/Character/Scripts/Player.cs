using System.Collections;
using UnityEngine;
using System;
using LOK1game.Game;

namespace LOK1game.PlayerDomain
{
    [RequireComponent(typeof(PlayerMovement), typeof(PlayerCamera), typeof(PlayerState))]
    [RequireComponent(typeof(Health), typeof(PlayerInteraction), typeof(PlayerWeaponManager))]
    public class Player : Pawn, IDamagable
    {
        public event Action OnRespawned;
        public event Action OnDeath;
        public event Action OnTakeDamage;
        public event Action OnStaminaOut;
        public event Action OnStaminaRecovered;

        public PlayerMovement Movement { get; private set; }
        public PlayerCamera Camera { get; private set; }
        public PlayerState State { get; private set; }
        public Health Health { get; private set; }
        public PlayerInteraction Interaction { get; private set; }
        public PlayerWeaponManager WeaponManager { get; private set; }

        public bool IsDead { get; private set; }

        public FirstPersonArms FirstPersonArms => _firstPersonArms;
        [SerializeField] private FirstPersonArms _firstPersonArms;

        [SerializeField] private Vector3 _crouchEyePosition;
        private Vector3 _defaultEyePosition;

        [SerializeField] private float _minimalSpeedToSlide = 7f;

        
        public float RespawnTime => _respawnTime;

        [SerializeField] private float _respawnTime;

        private float _targetTilt;

        [Space]
        [SerializeField] private float _maxSprintTime = 1f;
        [SerializeField] private float _staminaRecoveryTime = 1f;

        private float _stamina;
        private float _currentStaminaRecovery;

        private bool _staminaIsOut;

        private void Awake()
        {
            Health = GetComponent<Health>();
            Movement = GetComponent<PlayerMovement>();
            Camera = GetComponent<PlayerCamera>();
            Camera.Construct(this);
            State = GetComponent<PlayerState>();
            Interaction = GetComponent<PlayerInteraction>();
            WeaponManager = GetComponent<PlayerWeaponManager>();

            Interaction.Construct(this);

            Movement.OnLand += OnLand;
            Movement.OnJump += OnJump;
            Movement.OnStartCrouch += OnStartCrouching;
            Movement.OnStopCrouch += OnStopCrouching;
            Movement.OnStartSlide += OnStartSliding;
            Movement.OnStartSprint += OnStartSprint;
            Movement.OnStopSprint += OnStopSprint;

            _defaultEyePosition = Camera.GetCameraTransform().localPosition;
        }

        private void OnDestroy()
        {
            Movement.OnLand -= OnLand;
            Movement.OnJump -= OnJump;
            Movement.OnStartCrouch -= OnStartCrouching;
            Movement.OnStopCrouch -= OnStopCrouching;
            Movement.OnStartSlide -= OnStartSliding;

            Movement.OnStartSprint -= OnStartSprint;
            Movement.OnStopSprint -= OnStopSprint;
        }

        private void Start()
        {
            playerType = EPlayerType.View;

            _stamina = _maxSprintTime;

            WeaponManager.Construct(this);
            WeaponManager.EquipSlot(0);
        }

        private void Update()
        {
            var cameraRotation = Camera.GetCameraTransform().eulerAngles;

            Movement.DirectionTransform.rotation = Quaternion.Euler(0f, cameraRotation.y, 0f);

            Camera.Tilt = Mathf.Lerp(Camera.Tilt, _targetTilt, Time.deltaTime * 8f);

            if (State.IsSprinting && _staminaIsOut == false)
            {
                _stamina -= Time.deltaTime;

                if (_stamina < 0f)
                {
                    Movement.StopSprint();

                    _staminaIsOut = true;
                    OnStaminaOut?.Invoke();
                }
            }

            if (_staminaIsOut)
            {
                _currentStaminaRecovery += Time.deltaTime;

                if (_currentStaminaRecovery >= _staminaRecoveryTime)
                {
                    _currentStaminaRecovery = 0f;
                    _stamina = _maxSprintTime;

                    _staminaIsOut = false;
                    OnStaminaRecovered?.Invoke();
                }
            }
        }

        public override void OnInput(object sender)
        {
            if (IsDead)
                return;

            var inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            Camera.OnInput(this);
            Movement.SetAxisInput(inputAxis);

            #region movement inputs
            if (Input.GetKeyDown(KeyCode.Space))
                Movement.Jump();

            if (Input.GetKeyDown(KeyCode.C))
                Movement.StartCrouch();
                

            if (Input.GetKeyUp(KeyCode.C))
                if(Movement.CanStand())
                    Movement.StopCrouch();

            if (Input.GetKeyDown(KeyCode.LeftShift) && State.OnGround && _staminaIsOut == false)
                Movement.StartSprint();
            else if (Input.GetKeyUp(KeyCode.LeftShift))
                Movement.StopSprint();

            #endregion

            if (Input.GetKeyDown(KeyCode.U))
                TakeDamage(new Damage(15));

            Interaction.OnInput(this);
            WeaponManager.OnInput();
        }

        private void OnLand(float yVelocity)
        {
            yVelocity = Mathf.Max(yVelocity, -12);

            Camera.AddCameraOffset(0.12f * -yVelocity * Vector3.down);
            Camera.TriggerRecoil(new Vector3(-1f, 0f, 1f));
        }

        private void OnJump()
        {
            Camera.AddCameraOffset(Vector3.up * 0.35f);

            GetPlayerLogger().Push("Jump!", this);
        }

        private void OnStartCrouching()
        {
            Camera.DesiredPosition = _crouchEyePosition;
            _targetTilt = 0;
        }

        private void OnStartSliding()
        {
            Camera.DesiredPosition = _crouchEyePosition;
            _targetTilt = -3;
        }

        private void OnStopCrouching()
        {
            Camera.DesiredPosition = _defaultEyePosition;
            _targetTilt = 0;
        }

        private void OnStartSprint()
        {
            Camera.SetTargetFov(Camera.GetDefaultFov() +  Camera.GetDefaultFov() * 0.15f);
        }

        private void OnStopSprint()
        {
            Camera.ResetFov();
        }

        public void TakeDamage(Damage damage)
        {
            if (IsDead || damage.Value <= 0)
                return;

            Death();
        }


        private void Death()
        {
            if (IsDead)
                return;

            GetPlayerLogger().Push($"Player dead at {transform.position}");

            IsDead = true;
            Movement.Rigidbody.isKinematic = true;
            Movement.PlayerCollider.enabled = false;

            OnDeath?.Invoke();

            StartCoroutine(RespawnRoutine(BaseGameMode.GetRandomSpawnPointPosition(true)));
        }


        private void Respawn(Vector3 respawnPosition)
        {
            Debug.DrawRay(respawnPosition, Vector3.up * 2f, Color.yellow, _respawnTime + 1f, false);

            StartCoroutine(RespawnRoutine(respawnPosition));
        }

        private IEnumerator RespawnRoutine(Vector3 respawnPosition)
        {
            yield return new WaitForSeconds(_respawnTime);

            IsDead = false;

            Movement.Rigidbody.isKinematic = false;

            Movement.PlayerCollider.enabled = true;
            Movement.Rigidbody.linearVelocity = Vector3.zero;

            if (State.IsCrouching)
                Movement.StopCrouch();

            Health.ResetHealth();

            transform.position = respawnPosition;

            OnRespawned?.Invoke();
        }
    }
}