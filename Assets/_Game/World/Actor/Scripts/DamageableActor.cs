using UnityEngine;
using UnityEngine.Events;

namespace LOK1game
{
    [RequireComponent(typeof(Health))]
    public class DamageableActor : MonoBehaviour, IDamagable
    {
        public UnityEvent OnDeath;
        public UnityEvent OnDamage;

        public Health Health {  get; private set; }

        private void Awake()
        {
            Health = GetComponent<Health>();
        }

        public void TakeDamage(Damage damage)
        {
            if (Health.Hp <= 0)
                return;

            Health.ReduceHealth(damage.Value);
            OnDamage?.Invoke();
            
            if (Health.Hp <= 0)
                OnDeath?.Invoke();
        }
    }
}
