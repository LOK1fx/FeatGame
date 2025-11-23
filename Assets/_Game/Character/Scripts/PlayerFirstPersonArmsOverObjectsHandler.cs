using LOK1game.Tools;
using UnityEngine;

namespace LOK1game
{
    public class PlayerFirstPersonArmsOverObjectsHandler : MonoBehaviour
    {
        [SerializeField] private PlayerDomain.Player _player;
        [SerializeField] private Vector3 _firstPersonScale = Vector3.one * 0.15f;

        private Vector3 _initialScale;

        private void Awake()
        {
            _initialScale = transform.localScale;

            DebugUtility.AssertNotNull(_player);
        }

        private void Start()
        {
            if (_player.IsLocallyControlled)
                EnterFirstPerson();
        }

        public void EnterFirstPerson()
        {
            transform.localScale = _firstPersonScale;
        }

        public void ExitFirstPerson()
        {
            transform.localScale = _initialScale;
        }
    }
}
