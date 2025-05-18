using UnityEngine;

namespace LOK1game.UI
{
    public class UILevelCellsSpawner : MonoBehaviour
    {
        [SerializeField] private UILevelCell _cellPrefab;

        private void Start()
        {
            foreach (var level in LevelManager.LevelsData)
            {
                var cell = Instantiate(_cellPrefab, transform);

                cell.Initialize(level);
            }
        }
    }
}
