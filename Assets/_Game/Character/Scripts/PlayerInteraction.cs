using System;
using System.Collections;
using UnityEngine;

namespace LOK1game.PlayerDomain
{
    public class PlayerInteraction : MonoBehaviour, IInputabe
    {
        public event Action OnInteractionFound;
        public event Action OnInteractionLost;

        [SerializeField] private KeyCode _actionKey = KeyCode.F;

        [Space]
        [SerializeField] private LayerMask _interactableLayer;
        [SerializeField] private float _distance;

        private Transform _cameraTransform;
        private Player _owner;

        private IInteractable _currentInteractable;

        public void Construct(Player player)
        {
            _owner = player;
            _cameraTransform = player.Camera.GetCameraTransform();

            StartCoroutine(InteractionFindingRoutine());
        }

        public void OnInput(object sender)
        {
            if (_currentInteractable == null)
                return;

            if (Input.GetKeyDown(_actionKey))
                _currentInteractable.OnInteract(_owner);
        }

        private IEnumerator InteractionFindingRoutine()
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

            yield return new WaitForSeconds(0.1f);

            StartCoroutine(InteractionFindingRoutine());
        }
    }
}
