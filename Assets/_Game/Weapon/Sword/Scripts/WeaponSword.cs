using LOK1game.PlayerDomain;
using UnityEngine;

namespace LOK1game
{
    public class WeaponSword : WeaponBase
    {
        public override EWeaponId Id => EWeaponId.Sword;

        private uint _attackNum = 1;

        public override void AltUse()
        {
            FireAltUsed();
        }

        public override void Use()
        {
            Debug.Log("Sword make a splash attack");

            _attackNum++;

            if (_attackNum % 2 == 0)
            {
                animator.SetTrigger("Attack");
            }
            else
            {
                animator.SetTrigger("Attack02");
            }

            FireUsed();
        }

        protected override void OnDequip()
        {
            
        }

        protected override void OnEquip()
        {
            Player.FirstPersonArms.OverrideAnimatior(AnimController);
        }
    }
}
