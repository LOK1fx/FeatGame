using LOK1game.PlayerDomain;
using UnityEngine;

namespace LOK1game
{
    public class PlayerLegs : MonoBehaviour
    {
        [SerializeField] private Player _player;
        [SerializeField] private Vector3 _hiddenOffset = Vector3.back;

        private Vector3 _initialPosition;
        private Vector3 _targetPosition;

        private void Awake()
        {
            _initialPosition = transform.localPosition;

            transform.localPosition += _hiddenOffset;
            _targetPosition = transform.localPosition;

            _player.Movement.OnStartSlide += OnStartSlide;
            _player.Movement.OnStartCrouch += OnStartCrouch;
            _player.Movement.OnStopCrouch += OnStopCrouch;
        }

        private void OnDestroy()
        {
            _player.Movement.OnStartSlide -= OnStartSlide;
            _player.Movement.OnStartCrouch -= OnStartCrouch;
            _player.Movement.OnStopCrouch -= OnStopCrouch;
        }

        private void Update()
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, _targetPosition, Time.deltaTime * 8f);
            transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, transform.eulerAngles.z);
        }

        private void OnStopCrouch()
        {
            _targetPosition = _initialPosition + _hiddenOffset;
        }

        private void OnStartCrouch()
        {
            _targetPosition = _initialPosition + _hiddenOffset;
        }

        private void OnStartSlide()
        {
            _targetPosition = _initialPosition;
        }
    }
}
