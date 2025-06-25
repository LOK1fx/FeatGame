using UnityEngine;

namespace LOK1game
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicalActor : Actor, IDamagable
    {
        public bool CanGoInSleepMode => _canGoInSleepMode;
        [SerializeField] private bool _canGoInSleepMode = true;
        [SerializeField] private float _sleepDelay = 2f;
        [SerializeField] private float _minWakeUpImpulse = 0.1f;

        public Rigidbody Rigidbody => _rigidbody;
        [SerializeField] private Rigidbody _rigidbody;

        private float _lastInteractionTime;
        private bool _isSleeping;
        private Collider _collider;

        private void OnDrawGizmosSelected()
        {
            if (_collider == null)
                return;

            Gizmos.color = _isSleeping ? Color.blue : new Color(1f, 0.5f, 0f);
            Gizmos.DrawWireCube(_collider.bounds.center, _collider.bounds.size);
        }

        private void Awake()
        {
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody>();

            _collider = GetComponent<Collider>();
            _lastInteractionTime = Time.time;
        }

        public override void ApplicationUpdate()
        {
            if (!_canGoInSleepMode || _isSleeping)
                return;

            if (Time.time - _lastInteractionTime > _sleepDelay)
                PutToSleep();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!IsSignificantCollision(collision))
                return;

            if (_isSleeping)
                WakeUp();
            
            _lastInteractionTime = Time.time;
        }

        private void OnCollisionStay(Collision collision)
        {
            if (!IsSignificantCollision(collision))
                return;

            if (_isSleeping)
                WakeUp();
            
            _lastInteractionTime = Time.time;
        }

        private bool IsSignificantCollision(Collision collision)
        {
            float totalImpulse = 0f;
            
            for (int i = 0; i < collision.contactCount; i++)
            {
                totalImpulse += collision.GetContact(i).impulse.magnitude;
            }

            return totalImpulse > _minWakeUpImpulse;
        }

        private void PutToSleep()
        {
            _isSleeping = true;
            _rigidbody.Sleep();
        }

        private void WakeUp()
        {
            _isSleeping = false;
            _rigidbody.WakeUp();
            _lastInteractionTime = Time.time;
        }

        public void TakeDamage(Damage damage)
        {
            if (_isSleeping)
                WakeUp();

            _rigidbody.AddForceAtPosition(damage.GetHitDirection(true) * damage.PhysicalForce, damage.HitPoint, ForceMode.Impulse);
            _lastInteractionTime = Time.time;
        }
    }
}
