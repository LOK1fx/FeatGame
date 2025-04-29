using LOK1game.PlayerDomain;
using UnityEngine;
using UnityEngine.Audio;

namespace LOK1game
{
    [RequireComponent(typeof(Player), typeof(AudioSource))]
    public class PlayerAudio : MonoBehaviour
    {
        [SerializeField] private AudioClip[] _surfaceJumps;
        [SerializeField] private AudioClip[] _slightJumps;
        [SerializeField] private AudioClip[] _powerJumps;
        [SerializeField] private AudioClip[] _landClips;
        [SerializeField] private AudioClip _staminaOutClip;

        private AudioSource _source;
        private Player _player;

        private void Awake()
        {
            _player = GetComponent<Player>();
            _source = GetComponent<AudioSource>();

            _player.Movement.OnJump += OnJump;
            _player.OnStaminaOut += OnStaminaOut;
            _player.Movement.OnLand += OnLand;
        }

        private void OnDestroy()
        {
            _player.Movement.OnJump -= OnJump;
            _player.OnStaminaOut -= OnStaminaOut;
            _player.Movement.OnLand -= OnLand;
        }

        private void OnJump()
        {
            if (_player.Movement.GetSpeed() > 7)
                _source.PlayOneShot(_powerJumps[Random.Range(0, _powerJumps.Length)]);
            else
                _source.PlayOneShot(_slightJumps[Random.Range(0, _slightJumps.Length)]);

            if (_player.State.OnGround)
                _source.PlayOneShot(_surfaceJumps[Random.Range(0, _surfaceJumps.Length)], 1.5f);
        }

        private void OnStaminaOut()
        {
            _source.PlayOneShot(_staminaOutClip, 1.25f);
        }

        private void OnLand(float obj)
        {
            _source.PlayOneShot(_landClips[Random.Range(0, _landClips.Length)], 1.5f);
        }
    }
}
