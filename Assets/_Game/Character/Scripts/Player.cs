using System.Collections;
using UnityEngine;
using System;
using LOK1game.Game;
using LOK1game.Utility;

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
        public float MaxStamina => _maxSprintTime;

        [SerializeField] private float _staminaRecoveryTime = 1f;

        public float Stamina { get; private set; }

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

            Stamina = _maxSprintTime;

            WeaponManager.Construct(this);
            WeaponManager.EquipSlot(0);
        }

        public override void ApplicationUpdate()
        {
            if (IsLocal == false)
                return;

            UpdateDirectionTransform();

            Camera.Tilt = Mathf.Lerp(Camera.Tilt, _targetTilt, Time.deltaTime * 8f);

            if (State.IsSprinting && _staminaIsOut == false)
            {
                Stamina -= Time.deltaTime;

                if (Stamina < 0f)
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
                    Stamina = _maxSprintTime;

                    _staminaIsOut = false;
                    OnStaminaRecovered?.Invoke();
                }
            }
        }

        private void UpdateDirectionTransform()
        {
            var cameraRotation = Camera.GetCameraTransform().eulerAngles;
            Movement.DirectionTransform.rotation = Quaternion.Euler(0f, cameraRotation.y, 0f);
        }

        public override void OnInput(object sender)
        {
            if (IsDead || IsLocal == false)
                return;

            var inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            Camera.OnInput(this);
            Movement.SetAxisInput(inputAxis);

            #region movement inputs
            if (Input.GetKeyDown(KeyCode.Space))
                Movement.Jump();

            if (Input.GetKeyDown(KeyCode.LeftControl))
                Movement.StartCrouch();
                

            if (Input.GetKeyUp(KeyCode.LeftControl))
                if(Movement.CanStand())
                    Movement.StopCrouch();

            if (Input.GetKeyDown(KeyCode.LeftShift) && State.OnGround && _staminaIsOut == false)
                Movement.StartSprint();
            else if (Input.GetKeyUp(KeyCode.LeftShift))
                Movement.StopSprint();

            #endregion

            Interaction.OnInput(this);
            WeaponManager.OnInput();
        }

        public bool TryGetPlayerController(out PlayerController controller)
        {
            if (Controller == null || Controller is PlayerController == false)
            {
                controller = null;
                return false;
            }

            controller = Controller as PlayerController;
            return true;
        }

        public override void ApplyPitch(float angle) => Debug.LogWarning("Player cant apply pitch.");
        public override void ApplyRoll(float angle) => Debug.LogWarning("Player cant apply roll.");
        public override void ApplyYaw(float angle)
        {
            Camera.ApplyYaw(angle);
            UpdateDirectionTransform();
        }

        public override IEnumerator OnActorDestroy()
        {
            Movement.StopSprint();
            Movement.SetAxisInput(Vector2.zero);

            GetPlayerLogger().Push("LocalPlayer destroy", this);

            Destroy(gameObject);

            yield return null;
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

            Health.ReduceHealth(damage.Value);
            Camera.TriggerRecoil(new Vector3(-25f, 18f, 5f));

            if (Health.Hp <= 0)
                Death();

            OnTakeDamage?.Invoke();
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

            var spawnPoint = BaseGameMode.GetRandomSpawnPoint(true);

            GetPlayerLogger().Push($"Player will respawn at {spawnPoint.Position}.");

            StartCoroutine(RespawnRoutine(spawnPoint));
        }

        private IEnumerator RespawnRoutine(CharacterSpawnPoint spawnPoint)
        {
            Debug.DrawRay(spawnPoint.Position, Vector3.up * 2f, Color.yellow, _respawnTime + 1f, false);

            yield return new WaitForSeconds(_respawnTime);

            IsDead = false;
            Movement.PlayerCollider.enabled = true;
            Movement.Rigidbody.linearVelocity = Vector3.zero;

            if (State.IsCrouching)
                Movement.StopCrouch();

            Health.ResetHealth();

            transform.position = spawnPoint.Position;
            ApplyYaw(spawnPoint.Yaw);
            
            Movement.Rigidbody.isKinematic = false;

            OnRespawned?.Invoke();
        }
    }
}