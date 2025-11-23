using LOK1game.Game;

namespace LOK1game.Tools
{
    public class GameObjectActivitySetterOnPauseState : GameObjectActivitySetterBase
    {
        protected override void OnGameStateChanged(EGameStateId newGameState)
        {
            if(newGameState != EGameStateId.Paused) { return; }

            targetGameObject.SetActive(activateObject);
        }
    }
}