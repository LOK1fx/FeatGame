using UnityEngine;
using UnityEngine.AI;

namespace LOK1game.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class AiAgent : Actor
    {
        public AiStateMachine StateMachine { get; private set; }
        public Transform Target { get; private set; }

        public float DefaultSpeed => _defaultSpeed;
        [SerializeField] private float _defaultSpeed;

        [SerializeField] private EAiStateId _startState;

        public NavMeshAgent NavAgent { get; private set; }

        private void Awake()
        {
            NavAgent = GetComponent<NavMeshAgent>();

            StateMachine = new AiStateMachine(this);
            InitializeStates();
            StateMachine.SetState(_startState);

            _defaultSpeed = NavAgent.speed;

            OnAwake();
        }

        protected abstract void OnAwake();

        private void Update()
        {
            StateMachine.Update();

            OnUpdate();
        }

        protected abstract void OnUpdate();

        protected abstract void InitializeStates();

        public virtual void Death()
        {
            StateMachine.SetState(EAiStateId.Death);
        }

        public void StopMovement()
        {
            NavAgent.speed = 0;
        }

        public void ResumeMovement()
        {
            NavAgent.speed = _defaultSpeed;
        }

        public void GetNavMeshAgent(out NavMeshAgent navAgent)
        {
            navAgent = NavAgent;
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (Application.isPlaying == true)
                StateMachine.GetCurrentState().OnGizmosLayer();

#if UNITY_EDITOR

            var stateText = StateMachine != null ? StateMachine.GetCurrentState().ToString() : "NoStateMachine";
            var style = new GUIStyle()
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.LowerCenter,
            };

            style.normal.textColor = Color.red;

            UnityEditor.Handles.Label(transform.position + Vector3.up * 1.5f,
                $"{gameObject.name}\n{stateText}", style);

#endif
        }
    }
}