using UnityEngine;

namespace LOK1game
{
    public class SpiderEnemy : EnemyBase
    {
        [SerializeField] private GameObject _particles;

        public override void OnTookDamage(Damage damage)
        {
            
        }

        protected override void InitializeStates()
        {
            StateMachine.AddState(new IdleState());
            StateMachine.AddState(new ChaseState());
            StateMachine.AddState(new CircleState());
            StateMachine.AddState(new AttackState(15));
        }

        protected override void OnAwake()
        {
            base.OnAwake();
        }

        protected override void OnDeath()
        {
            Instantiate(_particles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        protected override void OnUpdate()
        {
            
        }
    }
}
