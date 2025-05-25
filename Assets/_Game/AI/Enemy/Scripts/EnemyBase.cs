using LOK1game.AI;
using LOK1game.Game.Events;
using LOK1game.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LOK1game
{
    [RequireComponent(typeof(Health), typeof(TakeDamageEffect))]
    public abstract class EnemyBase : AiAgent, IDamagable, IPawn
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

        private EAiStateId _previousStateId;

        protected override void OnAwake()
        {
            _takeDamageEffect = GetComponent<TakeDamageEffect>();

            EventManager.AddListener<OnGameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<OnGameStateChangedEvent>(OnGameStateChanged);
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

        protected virtual void OnGameStateChanged(OnGameStateChangedEvent evt)
        {
            switch (evt.NewState)
            {
                case Game.EGameState.Gameplay:
                    StateMachine.SetState(_previousStateId);
                    break;
                case Game.EGameState.Paused:
                    _previousStateId = StateMachine.CurrentStateId;
                    StateMachine.SetState(EAiStateId.Nothing);
                    break;
                default:
                    break;
            }
        }

        public abstract void OnTookDamage(Damage damage);

        protected abstract void OnDeath();
    }
}
