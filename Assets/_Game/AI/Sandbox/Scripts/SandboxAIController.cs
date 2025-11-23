using UnityEngine;

namespace LOK1game
{
    public class SandboxAIController : Controller
    {
        protected override void Awake() { }

        public override void ApplicationUpdate()
        {
            InputContext.MovementInput = new Vector2(0, 1f);

            ControlledPawn?.OnInput(this, InputContext);
        }
    }
}
