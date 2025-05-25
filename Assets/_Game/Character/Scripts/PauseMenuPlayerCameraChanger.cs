using Cinemachine;
using UnityEngine.Events;
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

        [SerializeField] private Light _additionalLight;

        private CinemachineVirtualCamera _thisCamera;

        private void Awake()
        {
            _thisCamera = GetComponent<CinemachineVirtualCamera>();

            DebugUtility.AssertNotNull(_mainCamera);
            DebugUtility.AssertNotNull(_arms);
            DebugUtility.AssertNotNull(_additionalLight);

            _mainCameraDefaultPriority = _mainCamera.m_Priority;
            _additionalLight.gameObject.SetActive(false);
        }

        private void Start()
        {
            EventManager.AddListener<OnGameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDestroy()
        {
            EventManager.RemoveListener<OnGameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnGameStateChanged(OnGameStateChangedEvent evt)
        {
            switch (evt.NewState)
            {
                case Game.EGameState.Paused:
                    OnPause();
                    break;
                case Game.EGameState.Gameplay:
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
