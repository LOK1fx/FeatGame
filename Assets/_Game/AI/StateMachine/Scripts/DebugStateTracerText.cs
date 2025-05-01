using LOK1game.AI;
using TMPro;
using UnityEngine;

namespace LOK1game.AI
{
    [RequireComponent(typeof(AiAgent))]
    public class DebugStateTracerText : MonoBehaviour
    {
        [SerializeField] private TextMeshPro _text;

        private AiStateMachine _stateMachine;
        private AiAgent _agent;

        private void Start()
        {
            _agent = GetComponent<AiAgent>();
            _stateMachine = _agent.StateMachine;

            _stateMachine.OnStateChanged += OnStateChanged;
        }

        private void OnDestroy()
        {
            _stateMachine.OnStateChanged -= OnStateChanged;
        }

        private void OnStateChanged()
        {
            var state = _stateMachine.CurrentState.ToString();
            var actorName = _agent.name;

            _text.text = $"{actorName} \n{state}";
        }
    }
}
