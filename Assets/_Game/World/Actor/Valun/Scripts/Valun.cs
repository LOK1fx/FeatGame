using LOK1game.PlayerDomain;
using UnityEngine;

namespace LOK1game
{
    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody), typeof(AudioSource))]
    public class Valun : MonoBehaviour, IInteractable
    {
        [SerializeField] private float _moveSpeed = 8f;
        [SerializeField] private LayerMask _blockLayers;
        [SerializeField] private float _physicalForce = 6f;
        [SerializeField] private AudioClip _impactClip;

        private Vector3 _targetPosition;
        private BoxCollider _collider;
        private Rigidbody _rigidbody;
        private AudioSource _audioSource;

        private bool _isPhysical = false;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
            _rigidbody = GetComponent<Rigidbody>();
            _audioSource = GetComponent<AudioSource>();

            _targetPosition = transform.position;
        }

        private void Update()
        {
            if (_isPhysical)
                return;

            transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _moveSpeed);
        }

        public void MakePhysical()
        {
            _isPhysical = true;

            _rigidbody.useGravity = true;
            _rigidbody.isKinematic = false;
            _rigidbody.constraints = RigidbodyConstraints.None;
        }

        public void OnInteract(Player sender)
        {
            if (_isPhysical)
                return;

            var senderPosition = new Vector3(sender.transform.position.x, 0f, sender.transform.position.z);
            var selfPosition = new Vector3(transform.position.x, 0f, transform.position.z);

            var directionToPlayer = senderPosition - selfPosition;
            directionToPlayer.Normalize();
            var forwardDirection = transform.forward;
            var dotProduct = Vector3.Dot(forwardDirection, directionToPlayer);
            var crossProduct = Vector3.Cross(forwardDirection, directionToPlayer);

            if (dotProduct > 0.5f)
                Move(Vector3.back, sender);
            else if (dotProduct < -0.5f)
                Move(Vector3.forward, sender);
            else if (crossProduct.y > 0)
                Move(Vector3.left, sender);
            else if (crossProduct.y < 0)
                Move(Vector3.right, sender);
            else
                Debug.Log("Игрок находится прямо перед объектом или сзади, но слишком близко для определения направления.");
        }

        private void Move(Vector3 direction, Player sender)
        {
            if (Physics.Raycast(transform.position, direction, out var hit, _collider.size.x, _blockLayers))
                return;

            var targetPosition = transform.position + (direction * _collider.size.x);

            if (Physics.Raycast(targetPosition, Vector3.down, out var downHit, 1f, _blockLayers) == false)
            {
                MakePhysical();

                _rigidbody.AddForce(direction * _physicalForce, ForceMode.VelocityChange);

                sender.FirstPersonArms.Animator.SetTrigger("Attack");
                _audioSource.PlayOneShot(_impactClip);

                return;
            }

            _targetPosition = targetPosition;
            Debug.DrawRay(transform.position, direction, Color.green, 1f);

            sender.FirstPersonArms.Animator.SetTrigger("Attack");
            _audioSource.PlayOneShot(_impactClip);
        }
    }
}
