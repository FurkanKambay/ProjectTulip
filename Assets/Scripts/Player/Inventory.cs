using System;
using System.Linq;
using Game.Data.Interfaces;
using Game.Data.Items;
using Game.Data.Tiles;
using UnityEngine;

namespace Game.Player
{
    public class Inventory : MonoBehaviour
    {
        public IItem[] Hotbar { get; } = new IItem[9];
        public IItem HotbarSelected => Hotbar[HotbarSelectedIndex];
        public int HotbarSelectedIndex { get; private set; }

        public Pickaxe FirstPickaxe => Hotbar.OfType<Pickaxe>().First();

        public event Action<int> HotbarSelectionChanged;
        public event Action HotbarModified;

        private void OnHotbarSelected(int index)
        {
            if (index == HotbarSelectedIndex)
                return;

            HotbarSelectedIndex = index;
            HotbarSelectionChanged?.Invoke(index);
        }

        private void Awake()
        {
            Hotbar[0] = Resources.Load<Pickaxe>("Tools/Pickaxe");
            Hotbar[1] = Resources.Load<BlockTile>("Tiles/Stone");
            Hotbar[2] = Resources.Load<BlockTile>("Tiles/Dirt");
            Hotbar[3] = Resources.Load<BlockTile>("Tiles/Aquatic");

            HotbarModified?.Invoke();
        }

        private void OnEnable()
        {
            Input.Instance.HotbarSelected += OnHotbarSelected;
            HotbarSelectionChanged?.Invoke(HotbarSelectedIndex);
        }

        private void OnDisable() => Input.Instance.HotbarSelected -= OnHotbarSelected;
    }
}
