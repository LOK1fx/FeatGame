

using LOK1game.Utils;

namespace LOK1game.AI
{
    public enum EAiStateId
    {
        Chase,
        Idle,
        Death,
        Circle,
        Attack,
        Nothing
    }

    public interface IAiState
    {
        EAiStateId GetStateId();
        void Enter(AiAgent agent);
        void Update(AiAgent agent);
        void Exit(AiAgent agent);

        void OnGizmosLayer();
    }
}