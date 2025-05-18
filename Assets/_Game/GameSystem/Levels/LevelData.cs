using UnityEngine;
using System.Collections.Generic;

namespace LOK1game
{
    public enum ELevelCategory : byte
    {
        None = 0,
        MainGame,
        Bonus,

        Development = 100,
    }

    [CreateAssetMenu(fileName = "new LevelData", menuName = "LevelData")]
    public class LevelData : ScriptableObject
    {
        public Sprite LevelImage => _levelImage;
        public string DisplayName => _displayName;
        public EGameModeId LevelGameMode => _levelGameMode;
        public ELevelCategory Category => _category;
        public List<string> AdditiveScenes => _addativeScenes;
        public string MainSceneName => _mainSceneName;


        [SerializeField] private Sprite _levelImage;
        [SerializeField] private string _displayName;
        [SerializeField] private EGameModeId _levelGameMode = EGameModeId.None;
        [SerializeField] private ELevelCategory _category = ELevelCategory.None;

        [Space]
        [Header("Scene info")]
        [SerializeField] private string _mainSceneName;
        [SerializeField] private List<string> _addativeScenes = new();
    }
}