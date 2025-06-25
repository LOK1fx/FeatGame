using UnityEngine;

namespace LOK1game
{
    public class Rotator : MonoBehaviour, IApplicationUpdatable
    {
        [SerializeField] private Vector3 _rotation;

        private void OnEnable()
        {
            ApplicationUpdateManager.Register(this);
        }

        private void OnDisable()
        {
            ApplicationUpdateManager.Unregister(this);
        }

        public void ApplicationUpdate()
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + _rotation * Time.deltaTime);
        }
    }
}
