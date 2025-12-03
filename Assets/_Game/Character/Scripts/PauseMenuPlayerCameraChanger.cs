using Cinemachine;
using LOK1game.Game.Events;
using LOK1game.Tools;
using UnityEngine;

namespace LOK1game
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class PauseMenuPlayerCameraChanger : MonoBehaviour
    {
        [Space]
        [SerializeField] private CinemachineVirtualCamera _mainCamera;
        private int _mainCameraDefaultPriority;

        [SerializeField] private PlayerFirstPersonArmsOverObjectsHandler _arms;
        [SerializeField] private PlayerDomain.Player _player;

        [SerializeField] private Light _additionalLight;

        private CinemachineVirtualCamera _thisCamera;

        private void Awake()
        {
            _thisCamera = GetComponent<CinemachineVirtualCamera>();

            DebugUtility.AssertNotNull(_mainCamera);
            DebugUtility.AssertNotNull(_arms);
            DebugUtility.AssertNotNull(_additionalLight);
            DebugUtility.AssertNotNull(_player);
        }

        private void Start()
        {
            EventManager.AddListener<OnGameStateChangedEvent>(OnGameStateChanged);

            _mainCameraDefaultPriority = _player.Camera.DefaultCameraPriority;
            _additionalLight.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<OnGameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnGameStateChanged(OnGameStateChangedEvent evt)
        {
            if (_player.IsOwner == false)
                return;

            Debug.Log("On State changed");

            switch (evt.NewState)
            {
                case Game.EGameStateId.Paused:
                    OnPause();
                    break;
                case Game.EGameStateId.Gameplay:
                    OnResume();
                    break;
            }
        }

        private void OnResume()
        {
            _mainCamera.Priority = _mainCameraDefaultPriority;
            _thisCamera.Priority = 0;

            _additionalLight.gameObject.SetActive(false);
            _arms.EnterFirstPerson();
        }

        private void OnPause()
        {
            _thisCamera.Priority = _mainCameraDefaultPriority + 1;
            _mainCamera.Priority = 0;

            _additionalLight.gameObject.SetActive(true);
            _arms.ExitFirstPerson();
        }
    }
}
