using LOK1game.PlayerDomain;
using UnityEngine;

namespace LOK1game
{
    public class WeaponSword : WeaponBase
    {
        public override void AltUse()
        {
            FireAltUsed();
        }

        public override void Use()
        {
            Debug.Log("Sword make a splash attack");

            FireUsed();
        }

        protected override void OnDequip()
        {
            
        }

        protected override void OnEquip()
        {
            
        }
    }
}
