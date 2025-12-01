namespace LOK1game
{
    public interface IPawn : IInputabe
    {
        //Controller Controller { get; }
        Controller<Pawntype> GetController<Pawntype>() where Pawntype : IPawn;
        void OnPocces<Pawntype>(Controller<Pawntype> sender, PlayerCharacterInputContext inputContext) where Pawntype: IPawn;
        void OnUnpocces(PlayerCharacterInputContext inputContext);
    }
}