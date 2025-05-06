using UnityEngine;

namespace LOK1game
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicalActor : Actor, IDamagable
    {
        public Rigidbody Rigidbody => _rigidbody;
        [SerializeField] private Rigidbody _rigidbody;

        private void Awake()
        {
            if (_rigidbody != null)
                return;

            _rigidbody = GetComponent<Rigidbody>();
        }

        public void TakeDamage(Damage damage)
        {
            _rigidbody.AddForceAtPosition(damage.GetHitDirection(true) * damage.PhysicalForce, damage.HitPoint, ForceMode.Impulse);
        }
    }
}
