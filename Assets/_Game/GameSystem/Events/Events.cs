namespace LOK1game.Game.Events
{
    public static class Events
    {
        
    }

    public class OnGameStateChangedEvent : GameEvent
    {
        public readonly EGameStateId PreviousState;
        public readonly EGameStateId NewState;

        public OnGameStateChangedEvent(EGameStateId previousState, EGameStateId newState)
        {
            PreviousState = previousState;
            NewState = newState;
        }
    }

    public class OnProjectContextInitializedEvent : SystemEvent
    {
        public readonly ProjectContext ProjectContext;

        public OnProjectContextInitializedEvent(ProjectContext projectContext)
        {
            ProjectContext = projectContext;
        }
    }

    public class OnDevConsoleStateChangedEvent : SystemEvent
    {
        public readonly bool Enabled;

        public OnDevConsoleStateChangedEvent(bool enabled)
        {
            Enabled = enabled;
        }
    }
}