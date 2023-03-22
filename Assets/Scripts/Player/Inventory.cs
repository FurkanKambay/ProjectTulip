using Game.Items;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Player
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance { get; private set; }

        public IUsable HotbarSelected { get; private set; }

        private readonly IUsable[] hotbar = new IUsable[9];

        private void OnHotbarSelected(int index) => HotbarSelected = hotbar[index];

        private void Start()
        {
            hotbar[0] = new Pickaxe();
            hotbar[1] = new Block { Tile = Resources.Load<Tile>("Tiles/stone") };
            hotbar[2] = new Block { Tile = Resources.Load<Tile>("Tiles/dirt") };
            hotbar[3] = new Block { Tile = Resources.Load<Tile>("Tiles/sand") };
            hotbar[4] = new Block { Tile = Resources.Load<Tile>("Tiles/ice") };

            HotbarSelected = hotbar[0];
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
