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

        public const string WorldName = "World";

        public WorldData World => loadedWorld;

        private readonly WorldSaveDictionary worldSaves = new();
        private WorldData loadedWorld;

        private void Awake() => ReturnToMainMenu();

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
        public async Awaitable CreateNewWorld(string worldName = WorldName)
        {
            if (!CanSaveWorld(worldName))
                return;

            WorldData generatedWorld = await worldGenerator.GenerateWorldAsync(worldName);
            worldSaves[worldName] = generatedWorld;
            // TODO: save generated data
        }

        [Button]
        public void LoadWorld(string worldName = WorldName)
        {
            if (!CanLoadWorld(worldName))
                return;

            loadedWorld = worldSaves[worldName];
            OnProvideWorld?.Invoke(loadedWorld);

            GameManager.SwitchTo(GameState.Playing);
        }

        [Button]
        public void DeleteWorld(string worldName = WorldName)
        {
            if (!CanLoadWorld(worldName))
                return;

            loadedWorld = null;
            worldSaves.Remove(worldName);
        }

        public bool CanSaveWorld(string worldName = WorldName) =>
            !string.IsNullOrWhiteSpace(worldName)
            && !worldSaves.ContainsKey(worldName);

        public bool CanLoadWorld(string worldName = WorldName) =>
            !string.IsNullOrWhiteSpace(worldName)
            && worldSaves.ContainsKey(worldName)
            && loadedWorld?.Name != worldName;
    }
}
