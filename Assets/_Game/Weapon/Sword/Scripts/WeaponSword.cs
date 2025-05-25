using LOK1game.PlayerDomain;
using UnityEngine;
using System.Collections;
using LOK1game.Tools;

namespace LOK1game
{
    public class WeaponSword : WeaponBase
    {
        public override EWeaponId Id => EWeaponId.Sword;

        [SerializeField] private Vector3 _damageSpherePosition;
        [SerializeField] private float _damageSphereRadius;
        [SerializeField] private TrailRenderer _trail;

        private uint _attackNum = 1;
        private bool _isAttacking = false;

        public override void AltUse()
        {
            FireAltUsed();
        }

        public override void Use()
        {
            if (_isAttacking)
                return;

            _attackNum++;

            if (_attackNum % 2 == 0)
                animator.SetTrigger("Attack");
            else
                animator.SetTrigger("Attack02");

            PlayRandomAttackClip();
            StartCoroutine(DelayedDamage());
        }

        private IEnumerator DelayedDamage()
        {
            _isAttacking = true;
            _trail.emitting = true;

            yield return new WaitForSeconds(AttackRate);

            DebugUtility.DrawSphere(GetDamageSpherePosition(), _damageSphereRadius, Color.red, AttackRate);

            var damagableColliders = Physics.OverlapSphere(GetDamageSpherePosition(), _damageSphereRadius,
                DamagableMask, QueryTriggerInteraction.Collide);

            foreach (var collider in damagableColliders)
            {
                if (collider.gameObject.TryGetComponent<IDamagable>(out var damagable))
                {
                    var playerCameraPosition = Player.Camera.GetCameraTransform().position;
                    var damage = new Damage()
                    {
                        Value = Damage,
                        Sender = Player,
                        PhysicalForce = Damage * 5f,
                        DamageType = EDamageType.Normal,

                        HitPoint = collider.ClosestPoint(playerCameraPosition),
                        HitNormal = collider.transform.position.GetDirectionTo(playerCameraPosition)
                    };

                    damagable.TakeDamage(damage);
                }
            }

            _isAttacking = false;
            _trail.emitting = false;

            FireUsed();
        }

        protected override void OnDequip()
        {
            _trail.emitting = false;
        }

        protected override void OnEquip()
        {
            Player.FirstPersonArms.OverrideAnimatior(AnimController);
            _trail.emitting = false;
        }

        private Vector3 GetDamageSpherePosition()
        {
            var camera = Player.Camera.GetCameraTransform();

            return camera.position + camera.TransformDirection(_damageSpherePosition);
        }
    }
}
