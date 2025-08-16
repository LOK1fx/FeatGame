using LOK1game.Tools;
using UnityEngine;

namespace LOK1game
{
    [RequireComponent(typeof(TakeDamageEffect), typeof(AudioSource), typeof(Animator))]
    public class TrainingDummy : Actor, IDamagable
    {
        private const string ANIM_TRIGGER_HURT = "Hurt";

        [SerializeField] private AudioClip[] _takeDamageClips;

        private AudioSource _source;
        private Animator _animator;
        private TakeDamageEffect _effect;

        private void Awake()
        {
            _source = GetComponent<AudioSource>();
            _effect = GetComponent<TakeDamageEffect>();
            _animator = GetComponent<Animator>();
        }

        public void TakeDamage(Damage damage)
        {
            _effect.PlayEffect();
            _source.PlayRandomClipOnce(_takeDamageClips);
            _animator.SetTrigger(ANIM_TRIGGER_HURT);
        }
    }
}
