using System;

namespace LOK1game.AI
{
    public class AiStateMachine
    {
        public event Action OnStateChanged;

        public AiStateId CurrentState { get; private set; }

        private IAiState[] _states;
        private AiAgent _agent;

        public AiStateMachine(AiAgent agent)
        {
            _agent = agent;

            var length = Enum.GetNames(typeof(AiStateId)).Length;

            _states = new IAiState[length];
        }

        public void Update()
        {
            GetState(CurrentState)?.Update(_agent);
        }

        public void AddState(IAiState state)
        {
            var index = (int)state.GetStateId();

            _states[index] = state;
        }

        public void SetState(AiStateId newStateId)
        {
            GetState(CurrentState)?.Exit(_agent);

            CurrentState = newStateId;

            GetState(newStateId)?.Enter(_agent);

            OnStateChanged?.Invoke();
        }

        public IAiState GetState(AiStateId stateId)
        {
            var index = (int)stateId;

            return _states[index];
        }
    }
}