using UnityEngine;

namespace LOK1game
{
    [ExecuteInEditMode]
    public class CharacterSpawnPointPreview : MonoBehaviour
    {
        [SerializeField] private CharacterSpawnPoint _parent;

        private void Update()
        {
            if ( _parent == null )
                return;

            transform.rotation = Quaternion.Euler(0f, _parent.transform.rotation.eulerAngles.y, 0f);
        }
    }
}
