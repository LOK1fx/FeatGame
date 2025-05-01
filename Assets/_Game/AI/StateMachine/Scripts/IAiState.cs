

using LOK1game.Utils;

namespace LOK1game.AI
{
    public enum AiStateId
    {
        Chase,
        Idle,
        Death,
        Circle,
        Attack
    }

    public interface IAiState
    {
        AiStateId GetStateId();
        void Enter(AiAgent agent);
        void Update(AiAgent agent);
        void Exit(AiAgent agent);
    }
}