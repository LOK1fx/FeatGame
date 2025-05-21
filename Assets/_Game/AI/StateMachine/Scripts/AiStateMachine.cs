using System;

namespace LOK1game.AI
{
    public class AiStateMachine
    {
        public event Action OnStateChanged;

        public EAiStateId CurrentStateId { get; private set; }

        private IAiState[] _states;
        private AiAgent _agent;

        public AiStateMachine(AiAgent agent)
        {
            _agent = agent;

            var length = Enum.GetNames(typeof(EAiStateId)).Length;

            _states = new IAiState[length];
        }

        public void Update()
        {
            GetState(CurrentStateId)?.Update(_agent);
        }

        public void AddState(IAiState state)
        {
            var index = (int)state.GetStateId();

            _states[index] = state;
        }

        public void SetState(EAiStateId newStateId)
        {
            GetState(CurrentStateId)?.Exit(_agent);

            CurrentStateId = newStateId;

            GetState(newStateId)?.Enter(_agent);

            OnStateChanged?.Invoke();
        }

        public IAiState GetState(EAiStateId stateId)
        {
            var index = (int)stateId;

            return _states[index];
        }

        public IAiState GetCurrentState()
        {
            return GetState(CurrentStateId);
        }
    }
}