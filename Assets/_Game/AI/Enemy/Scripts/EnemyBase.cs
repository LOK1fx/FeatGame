using LOK1game.AI;
using UnityEngine;

namespace LOK1game
{
    [RequireComponent(typeof(Health), typeof(TakeDamageEffect))]
    public abstract class EnemyBase : AiAgent, IDamagable
    {
        public Health Health => _health;
        [SerializeField] private Health _health;

        private TakeDamageEffect _takeDamageEffect;

        protected override void OnAwake()
        {
            _takeDamageEffect = GetComponent<TakeDamageEffect>();
        }


        public void TakeDamage(Damage damage)
        {
            _takeDamageEffect.Flash();
        }

        public abstract void OnTookDamage(Damage damage);
    }
}
