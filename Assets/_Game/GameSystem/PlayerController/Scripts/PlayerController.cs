namespace LOK1game
{
    public class PlayerController : Controller
    {
        protected override void Awake()
        {
            
        }

        public override void ApplicationUpdate()
        {
            ControlledPawn?.OnInput(this);
        }
    }
}