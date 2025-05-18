using LOK1game.AI;
using LOK1game.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LOK1game
{
    [RequireComponent(typeof(Health), typeof(TakeDamageEffect))]
    public abstract class EnemyBase : AiAgent, IDamagable
    {
        public event Action<string> OnDied;

        public bool IsDead => isDead;
        protected bool isDead;
        
        public Health Health => _health;
        [SerializeField] private Health _health;

        private TakeDamageEffect _takeDamageEffect;

        [Space]
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] private List<AudioClip> _gettingHurtClips;
        [SerializeField] private List<AudioClip> _deathClips;

        protected override void OnAwake()
        {
            _takeDamageEffect = GetComponent<TakeDamageEffect>();
        }


        public void TakeDamage(Damage damage)
        {
            _takeDamageEffect.PlayEffect();
            audioSource?.PlayRandomClipOnce(_gettingHurtClips);

            _health.ReduceHealth(damage.Value);

            if (_health.Hp <= 0 && isDead == false)
            {
                TryGetUniqueId(out var id);

                audioSource?.transform.SetParent(null, true);
                audioSource?.PlayRandomClipOnce(_deathClips);
                OnDied?.Invoke(id.ToString());

                OnDeath();
            }   
        }

        public abstract void OnTookDamage(Damage damage);

        protected abstract void OnDeath();
    }
}
