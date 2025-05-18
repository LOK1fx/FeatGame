#if UNITY_EDITOR

using UnityEditor;

#endif

using UnityEngine;

namespace LOK1game
{
    public class UniqueActorId : MonoBehaviour
    {
        public string Guid => _guid;

        [SerializeField] private string _guid;

        private void Awake()
        {
            if (string.IsNullOrEmpty(_guid))
                _guid = System.Guid.NewGuid().ToString();
        }

        public override string ToString()
        {
            return _guid;
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var text = string.IsNullOrEmpty(_guid) ? "No id!" : _guid;
            
            var style = new GUIStyle();
            style.normal.textColor = string.IsNullOrEmpty(_guid) ? Color.red : Color.white;
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = string.IsNullOrEmpty(_guid) ? 42 : 12;

            var position = transform.position + Vector3.up * 0.5f;

            Handles.Label(position, $"id:{text}", style);
#endif
        }
    }
}
