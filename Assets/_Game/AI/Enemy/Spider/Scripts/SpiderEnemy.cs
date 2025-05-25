using UnityEngine;

namespace LOK1game
{
    public class SpiderEnemy : EnemyBase
    {
        [SerializeField] private GameObject _particles;

        public override void OnInput(object sender)
        {
            
        }

        public override void OnTookDamage(Damage damage)
        {
            
        }

        protected override void InitializeStates()
        {
            StateMachine.AddState(new IdleState());
            StateMachine.AddState(new ChaseState());
            StateMachine.AddState(new CircleState());
            StateMachine.AddState(new AttackState(15));
            StateMachine.AddState(new NothingState());
        }

        protected override void OnAwake()
        {
            base.OnAwake();
        }

        protected override void OnDeath()
        {
            Destroy(Instantiate(_particles, transform.position, Quaternion.identity), 1f);
            Destroy(gameObject);
        }

        protected override void OnUpdate()
        {
            
        }
    }
}
