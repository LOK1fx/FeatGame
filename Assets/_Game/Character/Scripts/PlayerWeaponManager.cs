using System.Collections.Generic;
using UnityEngine;

namespace LOK1game.PlayerDomain
{
    public class PlayerWeaponManager : MonoBehaviour
    {
        public Player Player { get; private set; }
        public FirstPersonArms Arms { get; private set; }

        public int SelectedIndex { get; private set; } = 0;
        public WeaponBase SelectedWeapon { get; private set; }


        [SerializeField] private List<WeaponBase> _weaponPrefabs = new();

        private List<WeaponBase> _weapons = new(); 


        public void Construct(Player player)
        {
            Player = player;
            Arms = player.FirstPersonArms;
        }

        private void Start()
        {
            InitializePrefabs();
        }

        public void OnInput()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
                Use();
            else if (Input.GetKeyDown(KeyCode.Mouse1))
                AltUse();
        }

        public void EquipSlot(int index)
        {
            if (index >= _weapons.Count)
                return;

            if (SelectedWeapon.gameObject != null)
                SelectedWeapon.gameObject.SetActive(false);

            SelectedIndex = index;
            SelectedWeapon = _weapons[index];

            _weapons[index].gameObject.SetActive(true);
            _weapons[index].Equip();
        }

        public void Use()
        {
            if (SelectedWeapon == null)
                return;

            SelectedWeapon.Use();
        }

        public void AltUse()
        {
            if (SelectedWeapon == null)
                return;

            SelectedWeapon.AltUse();
        }

        private void InitializePrefabs()
        {
            foreach (var weapon in _weaponPrefabs)
            {
                var spawnedWeapon = Instantiate(weapon, Arms.RightHandSocket);

                spawnedWeapon.Bind(Player);
                spawnedWeapon.gameObject.SetActive(false);

                _weapons.Add(spawnedWeapon);
            }
        }
    }
}
