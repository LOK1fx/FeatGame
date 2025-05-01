using LOK1game.PlayerDomain;
using LOK1game.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LOK1game
{
    public enum EWeaponId : byte
    {
        None = 0,
        Sword = 10,
    }

    public abstract class WeaponBase : MonoBehaviour
    {
        public event Action OnUsed;
        public event Action OnAltUsed;
        public event Action OnEquiped;
        public event Action OnDequiped;


        public abstract EWeaponId Id { get; }
        public Player Player { get; private set; }


        [SerializeField] private int _damage;
        public int Damage => _damage;

        [SerializeField] private float _attackRate;
        public float AttackRate => _attackRate;

        [SerializeField] private LayerMask _damagableMask;
        public LayerMask DamagableMask => _damagableMask;


        [Space]
        [SerializeField] private AnimatorOverrideController _animController;
        public AnimatorOverrideController AnimController => _animController;

        [Space]
        [SerializeField] protected AudioSource _audioSource;
        [SerializeField] protected List<AudioClip> _attackClips = new();

        protected Animator animator { get; private set; }
        
        public void Bind(Player owner)
        {
            Player = owner;
            animator = owner.FirstPersonArms.Animator;
        }

        public void Equip()
        {
            OnEquip();

            OnEquiped?.Invoke();
        }

        public void Dequip()
        {
            OnDequip();

            OnDequiped?.Invoke();
        }

        protected abstract void OnEquip();
        protected abstract void OnDequip();

        public abstract void Use();
        public abstract void AltUse();

        protected void FireUsed() { OnUsed?.Invoke(); }
        protected void FireAltUsed() { OnAltUsed?.Invoke(); }

        protected void PlayRandomAttackClip()
        {
            if (_audioSource == null)
                return;

            _audioSource.PlayRandomClipOnce(_attackClips);
        }
    }
}
