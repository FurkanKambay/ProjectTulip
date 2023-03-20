using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Game
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance { get; private set; }

        public Tile HotbarSelectedTile { get; private set; }

        [SerializeField] private Tile[] hotbar = new Tile[9];

        private Inputs inputs;

        public void OnHotbar(InputAction.CallbackContext context)
        {
            int index = Convert.ToInt32(context.control.name);
            HotbarSelectedTile = hotbar[index];
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;
            HotbarSelectedTile = hotbar[0];
            inputs = new Inputs();
        }

        private void OnEnable()
        {
            inputs.Enable();
            inputs.Player.Hotbar.performed += OnHotbar;
        }

        private void OnDisable() => inputs.Disable();

        private void OnValidate()
        {
            if (hotbar.Length != 9)
                Array.Resize(ref hotbar, 9);
        }
    }
}
