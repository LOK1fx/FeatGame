using LOK1game.PlayerDomain;
using System;
using UnityEngine;

namespace LOK1game
{
    public abstract class WeaponBase : MonoBehaviour
    {
        public event Action OnUsed;
        public event Action OnAltUsed;
        public event Action OnEquiped;
        public event Action OnDequiped;

        public Player Player { get; private set; }


        [SerializeField] private int _damage;
        public int Damage => _damage;

        [SerializeField] private int _attackRate;
        public float AttackRate => _attackRate;


        [Space]
        [SerializeField] private AnimatorOverrideController _animController;
        public AnimatorOverrideController AnimController => _animController;

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
    }
}
