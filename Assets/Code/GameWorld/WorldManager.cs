using System;
using SaintsField.Playa;
using Tulip.Core;
using Tulip.Data;
using UnityEngine;

namespace Tulip.GameWorld
{
    [Serializable]
    public class WorldSaveDictionary : SerializableDictionary<string, WorldData>
    {
    }

    public class WorldManager : MonoBehaviour, IWorldProvider
    {
        public event IWorldProvider.ProvideWorldEvent OnProvideWorld;

        [Header("References")]
        [SerializeField] WorldGenerator worldGenerator;
        [SerializeField] StructureData playgroundStructure;

        public WorldData World => loadedWorld;

        private readonly WorldSaveDictionary worldSaves = new();
        private WorldData loadedWorld;

        private void Awake() => ReturnToMainMenu();

        private void OnEnable() => GameManager.OnGameStateChange += GameManager_StateChanged;
        private void OnDisable() => GameManager.OnGameStateChange -= GameManager_StateChanged;

        private async void GameManager_StateChanged(GameState oldState, GameState newState)
        {
            if (oldState is GameState.MainMenu && newState is GameState.Playing)
            {
                await CreateNewWorld("test");
                LoadWorld("test");
            }
            else if (newState is GameState.MainMenu)
                ReturnToMainMenu();
        }

        [Button]
        public void ReturnToMainMenu()
        {
            if (loadedWorld == playgroundStructure.WorldData)
                return;

            loadedWorld = playgroundStructure.WorldData;
            OnProvideWorld?.Invoke(loadedWorld);

            GameManager.SwitchTo(GameState.MainMenu);
        }

        [Button]
        public async Awaitable CreateNewWorld(string worldName)
        {
            if (!CanSaveName(worldName))
                return;

            WorldData generatedWorld = await worldGenerator.GenerateWorldAsync(worldName);
            worldSaves[worldName] = generatedWorld;
            // TODO: save generated data
        }

        [Button]
        public void LoadWorld(string worldName)
        {
            if (!CanLoadName(worldName))
                return;

            loadedWorld = worldSaves[worldName];
            OnProvideWorld?.Invoke(loadedWorld);

            GameManager.SwitchTo(GameState.Playing);
        }

        public bool CanSaveName(string worldName) =>
            !string.IsNullOrWhiteSpace(worldName)
            && !worldSaves.ContainsKey(worldName);

        public bool CanLoadName(string worldName) =>
            !string.IsNullOrWhiteSpace(worldName)
            && worldSaves.ContainsKey(worldName)
            && loadedWorld?.Name != worldName;
    }
}
