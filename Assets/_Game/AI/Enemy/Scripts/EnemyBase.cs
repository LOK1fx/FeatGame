using LOK1game.AI;
using UnityEngine;

namespace LOK1game
{
    [RequireComponent(typeof(Health), typeof(TakeDamageEffect))]
    public abstract class EnemyBase : AiAgent, IDamagable
    {
        public bool IsDead => isDead;
        protected bool isDead;
        
        public Health Health => _health;
        [SerializeField] private Health _health;

        private TakeDamageEffect _takeDamageEffect;

        protected override void OnAwake()
        {
            _takeDamageEffect = GetComponent<TakeDamageEffect>();
        }


        public void TakeDamage(Damage damage)
        {
            _takeDamageEffect.PlayEffect();

            _health.ReduceHealth(damage.Value);

            if (_health.Hp <= 0 && isDead == false)
                OnDeath();
        }

        public abstract void OnTookDamage(Damage damage);

        protected abstract void OnDeath();
    }
}
