using LOK1game.Game;

namespace LOK1game.Tools
{
    public class GameObjectActivitySetterOnGameplayState : GameObjectActivitySetterBase
    {
        protected override void OnGameStateChanged(EGameStateId newGameState)
        {
            if (newGameState != EGameStateId.Gameplay) { return; }

            targetGameObject.SetActive(activateObject);
        }
    }

}