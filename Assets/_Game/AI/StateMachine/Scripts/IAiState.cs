

namespace LOK1game.AI
{
    public enum AiStateId
    {
        Chase,
        Idle,
        Death,
        Atacking
    }

    public interface IAiState
    {
        AiStateId GetStateId();
        void Enter(AiAgent agent);
        void Update(AiAgent agent);
        void Exit(AiAgent agent);
    }
}