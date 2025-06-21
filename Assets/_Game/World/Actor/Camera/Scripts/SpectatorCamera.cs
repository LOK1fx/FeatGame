using LOK1game.Game.Events;
using UnityEngine;

namespace LOK1game
{
    [RequireComponent(typeof(Camera))]
    public class SpectatorCamera : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float _moveSpeed = 10f;
        [SerializeField] private float _fastMoveSpeed = 20f;
        [SerializeField] private float _mouseSensitivity = 2f;
        
        [Header("Limits")]
        [SerializeField] private float _minPitch = -89f;
        [SerializeField] private float _maxPitch = 89f;
        
        private Camera _camera;
        private float _pitch;
        private float _yaw;
        
        private bool _isInitialized = false;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            
            var euler = transform.eulerAngles;
            _pitch = euler.x;
            _yaw = euler.y;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            _isInitialized = true;
        }

        private void Start()
        {
            EventManager.AddListener<OnDevConsoleStateChangedEvent>(OnDevConsoleStateChanged);
        }

        private void OnDestroy()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            EventManager.RemoveListener<OnDevConsoleStateChangedEvent>(OnDevConsoleStateChanged);
        }

        private void Update()
        {
            if (!_isInitialized || Cursor.lockState == CursorLockMode.None)
                return;
            
            HandleInput();
            UpdateMovement();
            UpdateRotation();
        }
        
        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                ToggleCursor();
            
            if (Input.GetKeyDown(KeyCode.F12))
                App.Quit();
        }
        
        private void UpdateMovement()
        {
            var moveSpeed = Input.GetKey(KeyCode.LeftShift) ? _fastMoveSpeed : _moveSpeed;
            var deltaTime = Time.deltaTime;
            
            if (Input.GetKey(KeyCode.W))
                transform.position += transform.forward * moveSpeed * deltaTime;
            if (Input.GetKey(KeyCode.S))
                transform.position -= transform.forward * moveSpeed * deltaTime;
                
            if (Input.GetKey(KeyCode.A))
                transform.position -= transform.right * moveSpeed * deltaTime;
            if (Input.GetKey(KeyCode.D))
                transform.position += transform.right * moveSpeed * deltaTime;
                
            if (Input.GetKey(KeyCode.E))
                transform.position += Vector3.up * moveSpeed * deltaTime;
            if (Input.GetKey(KeyCode.Q))
                transform.position += Vector3.down * moveSpeed * deltaTime;
        }
        
        private void UpdateRotation()
        {
            if (Cursor.lockState != CursorLockMode.Locked) return;
            
            var mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            var mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;
            
            _yaw += mouseX;
            _pitch -= mouseY;
            _pitch = Mathf.Clamp(_pitch, _minPitch, _maxPitch);
            
            transform.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        }
        
        private void ToggleCursor()
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void OnDevConsoleStateChanged(OnDevConsoleStateChangedEvent evt)
        {
            if (evt.Enabled)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}
