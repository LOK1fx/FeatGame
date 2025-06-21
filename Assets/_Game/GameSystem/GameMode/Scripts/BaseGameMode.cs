using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using LOK1game.Tools;
using LOK1game.Utils;


#if UNITY_EDITOR

using LOK1game.Editor;
using UnityEditor;

#endif

public enum EGameModeState : ushort
{
    Starting = 1,
    Started,
    Ending,
    Ended,
}

namespace LOK1game.Game
{
    /// <summary>
    /// Базовый игровой режим со всеми нужными полями для создания
    /// игрового режима
    /// </summary>
    [Serializable]
    public abstract class BaseGameMode : MonoBehaviour, IGameMode, IDisposable
    {
        public EGameModeState State { get; protected set; }
        public List<GameObject> GameModeSpawnedObjects { get; private set; }

        public GameObject UiPrefab => _uiPrefab;
        public GameObject CameraPrefab => _cameraPrefab;
        public GameObject PlayerPrefab => _playerPrefab;
        public PlayerController PlayerController => _playerController;

        [SerializeField] private GameObject _uiPrefab;
        [SerializeField] private GameObject _cameraPrefab;
        [SerializeField] private GameObject _playerPrefab;
        [SerializeField] private PlayerController _playerController;

        private bool _isGameModeObjectListInitialized;

        public abstract EGameModeId Id { get; }

        public abstract IEnumerator OnEnd();
        public abstract IEnumerator OnStart();
        
        /// <summary>
        /// Создает объект, который сразу будет привязан к этому игровому режиму.
        /// </summary>
        /// <param name="gameObject">Объект который будет создан(префаб)</param>
        /// <param name="prefix">Префикс перед названием объекта {prefix}{objectName}{postfix}</param>
        /// <param name="postfix">Постфикс после названия объекта {prefix}{objectName}{postfix}</param>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <returns>Созданый объект</returns>
        protected T SpawnGameModeObject<T>(T gameObject, string prefix = "", string postfix = "") where T : Object
        {
            return SpawnGameModeObject<T>(gameObject, gameObject.name, prefix, postfix);
        }

        protected T SpawnGameModeObject<T>(T gameObject, string objectName, string prefix = "", string postfix = "") where T: Object
        {
            if (gameObject == null)
            {
                GetLogger().PushError($"Attempted to spawn null prefab '{objectName}' in {GetType().Name}");
                return null;
            }

            var newGameObject = Instantiate(gameObject);

            if (newGameObject == null)
            {
                GetLogger().PushError($"Failed to instantiate prefab '{objectName}' in {GetType().Name}");
                return null;
            }

            newGameObject.name = $"{prefix}{objectName}{postfix}";
            
            RegisterGameModeObject(newGameObject);

            return newGameObject;
        }

        protected PlayerController CreatePlayerController(IPawn controlledPawn)
        {
            if (PlayerController == null)
            {
                GetLogger().PushError($"PlayerController prefab is not assigned in {GetType().Name}");
                return null;
            }

            var playerController = Instantiate(PlayerController);

            if (playerController != null)
            {
                playerController.name = $"[{nameof(PlayerController)}]";
                playerController.SetControlledPawn(controlledPawn);

                RegisterGameModeObject(playerController.gameObject);
            }

            return playerController;
        }

        public static CharacterSpawnPoint GetRandomSpawnPoint(bool playerFlag)
        {
            var spawnPoints = FindObjectsByType<CharacterSpawnPoint>(FindObjectsSortMode.None).ToList();

            if (spawnPoints.Count == 0)
                return null;

            if (playerFlag)
                spawnPoints.RemoveAll(point => point.AllowPlayer == false);

            var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

            return spawnPoint;
        }

        public static Vector3 GetRandomSpawnPointPosition(int actorId = -1)
        {
            return GetRandomSpawnPointPosition(true, actorId);
        }
        
        public static Vector3 GetRandomSpawnPointPosition(bool playerFlag, int actorId = -1) 
        {
            var spawnPoint = GetRandomSpawnPoint(playerFlag);

            return spawnPoint.transform.position;
        }

        /// <summary>
        /// Привязывает объект к режиму
        /// </summary>
        /// <param name="gameObject">Объект</param>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <returns>Данный объект, прошедший привязку</returns>
        protected T RegisterGameModeObject<T>(T gameObject) where T: Object
        {
            if (gameObject == null)
            {
                GetLogger().PushError($"Attempted to register null object in {GetType().Name}");
                return null;
            }

            if (!_isGameModeObjectListInitialized)
                InitializeGameModeObjectList();

            var gameObjectAsGameObject = gameObject as GameObject;
            if (gameObjectAsGameObject == null)
            {
                GetLogger().PushError($"Failed to cast object {gameObject.name} to GameObject in {GetType().Name}");
                return gameObject;
            }

            GameModeSpawnedObjects.Add(gameObjectAsGameObject);
            DontDestroyOnLoad(gameObjectAsGameObject);

            return gameObject;
        }

        private void InitializeGameModeObjectList()
        {
            GameModeSpawnedObjects = new List<GameObject>();

            _isGameModeObjectListInitialized = true;
        }

        /// <summary>
        /// Удаляет все привязанные к режиму объекты
        /// </summary>
        /// <returns></returns>
        protected IEnumerator DestroyAllGameModeObjects()
        {
            if (_isGameModeObjectListInitialized && GameModeSpawnedObjects != null)
            {
                foreach (var obj in GameModeSpawnedObjects)
                {
                    if (obj != null)
                    {
                        if (obj.TryGetComponent<IDestroyableActor>(out var destroyableActor))
                            yield return destroyableActor.OnActorDestroy();
                        else
                            Destroy(obj as GameObject);
                    }

                    yield return new WaitForEndOfFrame();
                }
                
                GameModeSpawnedObjects.Clear();
            }
            else
            {
                GetLogger().PushWarning("GameModeObjectList is not initialized.");
            }

            yield return null;
        }

        public void Dispose()
        {
            Coroutines.StartRoutine(DestroyAllGameModeObjects());
        }

        protected LOK1gameLogger GetLogger()
        {
            return App.Loggers.GetLogger(ELoggerGroup.Application);
        }
    }
}