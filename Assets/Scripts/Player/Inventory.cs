using System;
using Game.Items;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Game.Player
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance { get; private set; }

        public IUsable HotbarSelected { get; private set; }

        private readonly IUsable[] hotbar = new IUsable[9];
        private InputActions input;

        public void OnHotbar(InputAction.CallbackContext context)
        {
            int index = Convert.ToInt32(context.control.name);
            HotbarSelected = hotbar[index - 1];
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;

            InitializeHotbar();
            input = new InputActions();
            input.Player.Hotbar.performed += OnHotbar;
        }

        private void InitializeHotbar()
        {
            hotbar[0] = new Pickaxe();
            hotbar[1] = new Block { Tile = Resources.Load<Tile>("Tiles/stone") };
            hotbar[2] = new Block { Tile = Resources.Load<Tile>("Tiles/dirt") };
            hotbar[3] = new Block { Tile = Resources.Load<Tile>("Tiles/sand") };
            hotbar[4] = new Block { Tile = Resources.Load<Tile>("Tiles/ice") };
            HotbarSelected = hotbar[0];
        }

        private void OnEnable() => input.Enable();
        private void OnDisable() => input.Disable();
    }
}
