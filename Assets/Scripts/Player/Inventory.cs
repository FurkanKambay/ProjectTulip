using System;
using System.Linq;
using Game.Data.Items;
using Game.Data.Tiles;
using Game.Helpers;
using UnityEngine;

namespace Game.Player
{
    public class Inventory : Singleton<Inventory>
    {
        public IUsable[] Hotbar { get; } = new IUsable[9];
        public IUsable HotbarSelected => Hotbar[hotbarSelectedIndex];
        public Pickaxe ActivePickaxe => Hotbar.First(u => u is Pickaxe) as Pickaxe;

        public event Action<int> HotbarSelectionChanged;
        public event Action HotbarModified;

        private int hotbarSelectedIndex;

        private void OnHotbarSelected(int index)
        {
            if (index == hotbarSelectedIndex)
                return;

            hotbarSelectedIndex = index;
            HotbarSelectionChanged?.Invoke(index);
        }

        private void Start()
        {
            Hotbar[0] = Resources.Load<Pickaxe>("Tools/Pickaxe");
            Hotbar[1] = Resources.Load<BlockTile>("Tiles/Stone");
            Hotbar[2] = Resources.Load<BlockTile>("Tiles/Dirt");
            Hotbar[3] = Resources.Load<BlockTile>("Tiles/Sand");
            Hotbar[4] = Resources.Load<BlockTile>("Tiles/Ice");

            HotbarModified?.Invoke();
        }

        private void OnEnable() => Input.Instance.HotbarSelected += OnHotbarSelected;
        private void OnDisable() => Input.Instance.HotbarSelected -= OnHotbarSelected;
    }
}
