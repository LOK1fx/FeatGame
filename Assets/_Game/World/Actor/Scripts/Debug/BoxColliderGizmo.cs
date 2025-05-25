using UnityEngine;

namespace LOK1game.DebugTools
{
    [RequireComponent(typeof(BoxCollider))]
    public class BoxColliderGizmo : MonoBehaviour
    {
        [SerializeField] private Color _gizmosColor = Color.green;
        [SerializeField] private bool _wire = true;

        private BoxCollider _collider;

        private void OnValidate()
        {
            if(_collider) { return; }

            _collider = GetComponent<BoxCollider>();
        }

        private void OnDrawGizmos()
        {
            var center = transform.position + transform.rotation * _collider.center;
            var scale = transform.localScale;
            var size = new Vector3(scale.x * _collider.size.x,
                scale.y * _collider.size.y, scale.z * _collider.size.z);

            Gizmos.color = _gizmosColor;
            
            Matrix4x4 rotationMatrix = Matrix4x4.TRS(center, transform.rotation, Vector3.one);
            Gizmos.matrix = rotationMatrix;

            if(_wire)
            {
                Gizmos.DrawWireCube(Vector3.zero, size);
            }
            else
            {
                Gizmos.DrawCube(Vector3.zero, size);
            }
            
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}