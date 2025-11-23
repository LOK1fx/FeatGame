using System;
using System.Collections;
using UnityEngine;

namespace LOK1game.PlayerDomain
{
    public class PlayerInteraction : MonoBehaviour
    {
        public event Action OnInteractionFound;
        public event Action OnInteractionLost;

        [SerializeField] private LayerMask _interactableLayer;
        [SerializeField] private float _distance;
        [SerializeField] private float _interactableCheckDelay = 0.15f;

        private Transform _cameraTransform;
        private Player _owner;

        private IInteractable _currentInteractable;
        private PlayerCharacterInputContext _inputContext;

        public void Construct(Player player)
        {
            _owner = player;
            _cameraTransform = player.Camera.GetCameraTransform();

            StartCoroutine(InteractionFindingRoutine());
        }

        public void BindInputContext(PlayerCharacterInputContext context)
        {
            _inputContext = context;
            _inputContext.OnInteractionButtonDown += TryToInteract;
        }

        public void UnbindInputContext(PlayerCharacterInputContext context)
        {
            _inputContext.OnInteractionButtonDown -= TryToInteract;
            _inputContext = null;
        }

        private void TryToInteract()
        {
            if (_currentInteractable == null)
                return;

            _currentInteractable.OnInteract(_owner);
        }

        private IEnumerator InteractionFindingRoutine()
        {
            while(true)
            {
                if (Physics.Raycast(_cameraTransform.position, _cameraTransform.forward,
                    out var hit, _distance, _interactableLayer, QueryTriggerInteraction.Collide))
                {
                    if (hit.collider.gameObject.TryGetComponent<IInteractable>(out var interactable))
                    {
                        if (_currentInteractable == null)
                            OnInteractionFound?.Invoke();

                        _currentInteractable = interactable;
                    }
                    else
                    {
                        if (_currentInteractable != null)
                            OnInteractionLost?.Invoke();

                        _currentInteractable = null;
                    }
                }
                else
                {
                    if (_currentInteractable != null)
                        _currentInteractable = null;
                    OnInteractionLost?.Invoke();
                }

                yield return new WaitForSeconds(_interactableCheckDelay);
            }
        }
    }
}
