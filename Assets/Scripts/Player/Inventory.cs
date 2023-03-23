using System.Linq;
using Game.Data.Items;
using Game.Data.Tiles;
using UnityEngine;

namespace Game.Player
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance { get; private set; }

        public IUsable[] Hotbar { get; } = new IUsable[9];
        public IUsable HotbarSelected { get; private set; }

        public Pickaxe ActivePickaxe => Hotbar.First(usable => usable is Pickaxe) as Pickaxe;

        private void OnHotbarSelected(int index) => HotbarSelected = Hotbar[index];

        private void Start()
        {
            Hotbar[0] = Resources.Load<Pickaxe>("Tools/Pickaxe");
            Hotbar[1] = Resources.Load<BlockTile>("Tiles/Stone");
            Hotbar[2] = Resources.Load<BlockTile>("Tiles/Dirt");
            Hotbar[3] = Resources.Load<BlockTile>("Tiles/Sand");
            Hotbar[4] = Resources.Load<BlockTile>("Tiles/Ice");

            HotbarSelected = Hotbar[0];
            Input.Instance.HotbarSelected += OnHotbarSelected;
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }
    }
}
