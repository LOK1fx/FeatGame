using UnityEngine;
using UnityEngine.Events;

namespace LOK1game
{
    public class Trigger : MonoBehaviour
    {
        public UnityEvent OnEnter;
        public UnityEvent OnExit;

        [SerializeField] private string _checkForTag = "Untagged";

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == _checkForTag)
                OnEnter?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == _checkForTag)
                OnExit?.Invoke();
        }
    }
}
