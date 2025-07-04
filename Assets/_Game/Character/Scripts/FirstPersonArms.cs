using UnityEngine;

namespace LOK1game
{
    [RequireComponent(typeof(Animator))]
    public class FirstPersonArms : MonoBehaviour
    {
        public Transform CameraSocket => _cameraSocket;
        public Transform RightHandSocket => _rightHandSocket;
        public Transform LeftHandSocket => _leftHandSocket;
        public Animator Animator { get; private set; }

        [SerializeField] private Transform _cameraSocket;
        [SerializeField] private Transform _rightHandSocket;
        [SerializeField] private Transform _leftHandSocket;

        private GameObject _rightHandObject;
        private GameObject _leftHandObject;

        private RuntimeAnimatorController _defaultController;

        private void Awake()
        {
            Animator = GetComponent<Animator>();
            _defaultController = Animator.runtimeAnimatorController;
        }

        public void AttachObjectToRightHand(GameObject gameObject)
        {
            _rightHandObject = AttachObjectToHand(gameObject, _rightHandSocket);
        }
        
        public void AttachObjectToLeftHand(GameObject gameObject)
        {
            _leftHandObject = AttachObjectToHand(gameObject, _leftHandSocket);
        }

        public void ClearRightHand()
        {
            if (_rightHandObject == null)
                return;

            Destroy(_rightHandObject);
            _rightHandObject = null;
        }

        public void ClearLeftHand()
        {
            if (_leftHandObject == null)
                return;

            Destroy(_leftHandObject);
            _leftHandObject = null;
        }

        public void OverrideAnimatior(AnimatorOverrideController controller)
        {
            if (controller == null)
                Animator.runtimeAnimatorController = _defaultController;
            else
                Animator.runtimeAnimatorController = controller;
        }


        private GameObject AttachObjectToHand(GameObject gameObject, Transform parent)
        {
            gameObject.transform.SetParent(parent, false);
            gameObject.transform.localScale = Vector3.one;
            gameObject.transform.localRotation = Quaternion.identity;

            return gameObject;
        }
    }
}