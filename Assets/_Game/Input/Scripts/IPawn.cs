namespace LOK1game
{
    public interface IPawn : IInputabe
    {
        Controller Controller { get; }
        void OnPocces(Controller sender, PlayerCharacterInputContext inputContext);
        void OnUnpocces(PlayerCharacterInputContext inputContext);
    }
}