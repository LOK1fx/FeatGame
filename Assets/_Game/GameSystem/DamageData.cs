using UnityEngine;

namespace LOK1game
{
    [CreateAssetMenu(fileName = "new DamageData", menuName = "Game/DamageData")]
    public class DamageData : ScriptableObject
    {
        public int Damage => _damage;
        public EDamageType Type => _type;
        public float PhysicalForce => _physicalForce;

        [SerializeField] private int _damage;
        [SerializeField] private EDamageType _type = EDamageType.Normal;
        [SerializeField] protected float _physicalForce;

        private void OnValidate()
        {
            _damage = Mathf.Max(_damage, 0);
        }
    }
}
