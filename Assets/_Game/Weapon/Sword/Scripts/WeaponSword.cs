using LOK1game.PlayerDomain;
using UnityEngine;
using System.Collections;

namespace LOK1game
{
    public class WeaponSword : WeaponBase
    {
        public override EWeaponId Id => EWeaponId.Sword;

        [SerializeField] private Vector3 _damageSpherePosition;
        [SerializeField] private float _damageSphereRadius;

        private uint _attackNum = 1;
        private bool _isAttacking = false;

        public override void AltUse()
        {
            FireAltUsed();
        }

        public override void Use()
        {
            if (_isAttacking) return;

            _attackNum++;

            if (_attackNum % 2 == 0)
            {
                animator.SetTrigger("Attack");
            }
            else
            {
                animator.SetTrigger("Attack02");
            }

            StartCoroutine(DelayedDamage());

            FireUsed();
        }

        private IEnumerator DelayedDamage()
        {
            _isAttacking = true;
            yield return new WaitForSeconds(AttackRate);

            var damagableColliders = Physics.OverlapSphere(GetDamageSpherePosition(), _damageSphereRadius,
                DamagableMask, QueryTriggerInteraction.Ignore);

            if (damagableColliders.Length > 0 && damagableColliders[0].gameObject.TryGetComponent<IDamagable>(out var damagable))
            {
                damagable.TakeDamage(new Damage(Damage, Player));
            }

            _isAttacking = false;
        }

        protected override void OnDequip()
        {
            
        }

        protected override void OnEquip()
        {
            Player.FirstPersonArms.OverrideAnimatior(AnimController);
        }

        private Vector3 GetDamageSpherePosition()
        {
            var camera = Player.Camera.GetCameraTransform();

            return camera.position + camera.TransformDirection(_damageSpherePosition);
        }

        private void OnDrawGizmos()
        {
            if (Player == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GetDamageSpherePosition(), _damageSphereRadius);
        }
    }
}
