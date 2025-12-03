using Cinemachine;
using LOK1game.PlayerDomain;
using UnityEngine;

namespace LOK1game.World
{
    public class Boat : Pawn, IInteractable
    {
        [SerializeField] private float _speed = 15f;
        [SerializeField] private CinemachineVirtualCamera _virtualCamera;

        private Player _player;
        private Controller<Pawn> _controller;

        public override void OnPocces<Pawntype>(Controller<Pawntype> sender, PlayerCharacterInputContext inputContext)
        {
            base.OnPocces(sender, inputContext);

            inputContext.OnInteractionButtonDown += OnInteractionButton;
            inputContext.OnFireButtonDown += InputContext_OnFireButtonDown;
            inputContext.OnAltFireButtonDown += InputContext_OnAltFireButtonDown;

            _controller = sender as Controller<Pawn>;
        }

        public override void OnUnpocces(PlayerCharacterInputContext inputContext)
        {
            base.OnUnpocces(inputContext);

            inputContext.OnInteractionButtonDown -= OnInteractionButton;

            inputContext.OnFireButtonDown -= InputContext_OnFireButtonDown;
            inputContext.OnAltFireButtonDown -= InputContext_OnAltFireButtonDown;
        }

        private void OnInteractionButton()
        {
            if (_player == null)
                return;

            _player.transform.parent = null;
            _player.transform.rotation = Quaternion.Euler(Vector3.zero);
            _player.transform.localScale = Vector3.one;
            _player.Movement.Rigidbody.isKinematic = false;

            _player.Camera.ResetPriority();
            _virtualCamera.Priority = 0;

            _controller.SetControlledPawn(_player, false);
        }

        private void InputContext_OnAltFireButtonDown()
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + Vector3.forward * 15f);
        }

        private void InputContext_OnFireButtonDown()
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles - Vector3.forward * 15f);
        }

        public override void OnInput(object sender, PlayerCharacterInputContext inputContext)
        {
            transform.position += _speed * inputContext.MovementInput.y * Time.deltaTime * transform.forward;
            transform.position += _speed * inputContext.MovementInput.x * Time.deltaTime * transform.right;
        }

        public void OnInteract(Player sender)
        {
            if (sender.TryGetPlayerController(out var controller) == false)
                return;

            controller.SetControlledPawn(this, true);

            sender.transform.localScale = Vector3.one;
            sender.transform.parent = transform;
            sender.Movement.Rigidbody.isKinematic = true;

            sender.Camera.SetPriority(0);
            _virtualCamera.Priority = 100;

            _player = sender;
        }
    }
}
