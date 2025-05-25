using UnityEngine;

namespace LOK1game
{
    public class PlayerFirstPersonArmsOverObjectsHandler : MonoBehaviour
    {
        [SerializeField] private Vector3 _firstPersonScale = Vector3.one * 0.15f;

        private Vector3 _initialScale;

        private void Awake()
        {
            _initialScale = transform.localScale;
        }

        private void Start()
        {
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
