using LOK1game.PlayerDomain;
using UnityEngine;

namespace LOK1game
{
    [RequireComponent(typeof(Player))]
    public class FirstPersonArmsPlayerAnimations : MonoBehaviour
    {
        private const string TRIGGER_JUMP = "Jump";
        private const string TRIGGER_DOUBLEJUMP = "DoubleJump";
        private const string BOOL_SPRINT = "IsSprinting";
        private const string FLOAT_SPEED = "Speed";

        [SerializeField] private FirstPersonArms _arms;

        private Player _player;
        private Animator _animator;

        private void Start()
        {
            _player = GetComponent<Player>();
            _animator = _arms.Animator;

            _player.Movement.OnJump += OnJump;
            _player.Movement.OnStartSprint += OnSprintStarted;
            _player.Movement.OnStopSprint += OnSprintStopped;
        }

        private void OnDestroy()
        {
            _player.Movement.OnJump -= OnJump;
            _player.Movement.OnStartSprint -= OnSprintStarted;
            _player.Movement.OnStopSprint -= OnSprintStopped;
        }

        private void OnJump()
        {
            if (_player.State.OnGround)
                _animator.SetTrigger(TRIGGER_JUMP);
            else
                _animator.SetTrigger(TRIGGER_DOUBLEJUMP);
        }

        private void Update()
        {
            _animator.SetFloat(FLOAT_SPEED, _player.Movement.GetSpeed());
        }


        private void OnSprintStopped()
        {
            _animator.SetBool(BOOL_SPRINT, false);
        }

        private void OnSprintStarted()
        {
            _animator.SetBool(BOOL_SPRINT, true);
        }
    }
}
