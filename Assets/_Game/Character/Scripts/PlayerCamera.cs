using UnityEngine;
using Cinemachine;

namespace LOK1game.PlayerDomain
{
    public class PlayerCamera : MonoBehaviour
    {
        public float Tilt;
        
        [SerializeField] private float _sensitivity = 16f;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private CinemachineVirtualCamera _camera;

        [Space]
        [SerializeField] private float _maxRightViewAngle = 30f;
        [SerializeField] private float _maxLeftViewAngle = 30f;

        [Space]
        [SerializeField] private float _maxUpViewAngle = 15f;
        [SerializeField] private float _maxDownViewAngle = 15f;

        [Space]
        [SerializeField] private float _fovChangeSpeed = 8f;
        private float _defaultFov = 65f;
        private float _targetFov;

        public Vector3 DesiredPosition;

        [SerializeField] private float _cameraOffsetResetSpeed = 7f;

        private Vector3 _cameraLerpOffset;

        [SerializeField] private Transform _recoilCamera;
        [SerializeField] private Transform _animationCamera;
        [SerializeField] private Vector3 _animationCameraRotationOffset;
        [SerializeField] private float _recoilCameraRotationSpeed;
        [SerializeField] private float _recoilCameraReturnSpeed;

        private Vector3 _recoilCameraRotation;
        private Vector3 _currentRecoilCameraRotation;

        private float _xRotation;
        private float _yRotation;

        private Player _player;

        public void Construct(Player player)
        {
            _player = player;
        }

        public void OnStartClient()
        {
            if (_player.IsOwner)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                if (Settings.TryGetSensivity(out var sensitivity))
                    _sensitivity = sensitivity;

                Settings.OnSensivityChanged += OnSensivityChanged;

                _camera.Priority = 100;
                _camera.ForceCameraPosition(_camera.transform.position, _camera.transform.rotation);
            }

            DesiredPosition = _cameraTransform.localPosition;
            _defaultFov = _camera.m_Lens.FieldOfView;
            _targetFov = _defaultFov;
        }

        private void Start()
        {
            
        }

        private void OnDestroy()
        {
            Settings.OnSensivityChanged -= OnSensivityChanged;
        }


        private void Update()
        {
            _camera.m_Lens.FieldOfView = Mathf.Lerp(_camera.m_Lens.FieldOfView, _targetFov, Time.deltaTime * _fovChangeSpeed);

            var targetRot = Quaternion.Euler(_yRotation, _xRotation, Tilt);

            _cameraTransform.localRotation = targetRot;

            _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, DesiredPosition + _cameraLerpOffset, Time.deltaTime * _cameraOffsetResetSpeed);
            _cameraLerpOffset = Vector3.Lerp(_cameraLerpOffset, Vector3.zero, Time.deltaTime * _cameraOffsetResetSpeed);

            var animationRotation = _player.FirstPersonArms.CameraSocket.localRotation;

            _animationCamera.localRotation = new Quaternion(animationRotation.x, animationRotation.z, animationRotation.y, animationRotation.w)
                * Quaternion.Euler(_animationCameraRotationOffset);
        }

        

        private void FixedUpdate()
        {
            _recoilCameraRotation = Vector3.Lerp(_recoilCameraRotation, Vector3.zero, _recoilCameraReturnSpeed * Time.deltaTime);
            _currentRecoilCameraRotation = Vector3.Slerp(_currentRecoilCameraRotation, _recoilCameraRotation, _recoilCameraRotationSpeed * Time.fixedDeltaTime);
            _recoilCamera.localRotation = Quaternion.Euler(_currentRecoilCameraRotation);
        }

        public void TriggerRecoil(Vector3 recoil)
        {
            _recoilCameraRotation += new Vector3(-recoil.x, Random.Range(-recoil.y, recoil.y), Random.Range(-recoil.z, recoil.z));
        }

        public void OnInput(object sender, PlayerCharacterInputContext inputContext)
        {
            _xRotation += inputContext.LookInput.x * GetSensivityMultiplier();
            _yRotation -= inputContext.LookInput.y * GetSensivityMultiplier();

            _xRotation = Mathf.Clamp(_xRotation, -_maxLeftViewAngle, _maxRightViewAngle);
            _yRotation = Mathf.Clamp(_yRotation, -_maxUpViewAngle, _maxDownViewAngle);

            _xRotation = ThreehoundredToZero(_xRotation);
            _yRotation = ThreehoundredToZero(_yRotation);
        }

        public void SetTargetFov(float fov)
        {
            _targetFov = fov;
        }

        public void SetFov(float fov)
        {
            _camera.m_Lens.FieldOfView = fov;
        }

        public void ResetFov()
        {
            _targetFov = _defaultFov;
        }

        private float GetSensivityMultiplier()
        {
            var multiplier = 1f;

            if(Application.isMobilePlatform)
                multiplier = 0.05f;

            return _sensitivity * multiplier * Time.fixedDeltaTime;
        }

        private float ThreehoundredToZero(float value)
        {
            if(value >= 360 || value <= -360)
            {
                return 0f;
            }
            else
            {
                return value;
            }
        }

        public void ApplyYaw(float angle)
        {
            _xRotation = angle;
        }

        public void AddCameraOffset(Vector3 offset)
        {
            _cameraLerpOffset += offset;
        }

        public Transform GetCameraTransform()
        {
            return _cameraTransform;
        }

        public Transform GetRecoilCameraTransform()
        {
            return _recoilCamera.transform;
        }

        public float GetDefaultFov()
        {
            return _defaultFov;
        }

        public float GetMouseInputScale()
        {
            return GetSensivityMultiplier();
        }

        private void OnSensivityChanged(float newSens)
        {
            _sensitivity = newSens;
        }
    }
}