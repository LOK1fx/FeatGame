using LOK1game.PlayerDomain;
using UnityEngine;

namespace LOK1game
{
    [RequireComponent(typeof(PlayerState))]
    public class CharacterFootsteps : MonoBehaviour
    {
        [SerializeField] private float _interval = 1.5f;
        [SerializeField] private AudioSource _source;
        [SerializeField] private AudioClip[] _footsteps;

        private PlayerState _state;
        private Vector3 _previousPosition;


        private void Awake()
        {
            _state = GetComponent<PlayerState>();
            _previousPosition = transform.position;
        }

        private void Update()
        {
            if (_state.OnGround == false || _state.IsSliding == true)
                return;

            if (Vector3.Distance(_previousPosition, transform.position) > _interval)
            {
                _previousPosition = transform.position;

                _source.PlayOneShot(_footsteps[Random.Range(0, _footsteps.Length)]);
            }
        }
    }
}
