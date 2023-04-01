using System;
using System.Linq;
using Game.Data;
using Game.Data.Interfaces;
using Game.Data.Items;
using UnityEngine;

namespace Game.Player
{
    public class Inventory : MonoBehaviour
    {
        public IItem[] Hotbar { get; private set; }
        public IItem HotbarSelected => Hotbar[HotbarSelectedIndex];
        public int HotbarSelectedIndex { get; private set; }
        public Pickaxe FirstPickaxe => Hotbar.OfType<Pickaxe>().First();

        [SerializeField] private HotbarData hotbarData;

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
            Hotbar = hotbarData.hotbar.Cast<IItem>().ToArray();
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
