using Game.Data;
using Game.Data.Interfaces;
using Game.Data.Tiles;
using Game.Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
    public class HotbarUI : MonoBehaviour
    {
        [SerializeField] Inventory inventory;

        private UIDocument document;
        private VisualElement root;

        private void OnHotbarModified()
        {
            ItemStack[] items = inventory.Items;
            for (int i = 0; i < items.Length; i++)
            {
                IItem item = items[i]?.Item;
                Image image = root[i].Q<Image>();
                image.sprite = item?.Icon;

                if (item == null) return;
                image.transform.scale = Vector3.one * item.IconScale;
                image.tintColor = item is BlockTile block ? block.color : Color.white;
            }
        }

        private void OnHotbarSelectionChanged(int index)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                if (i == index)
                    root[i].AddToClassList("selected");
                else
                    root[i].RemoveFromClassList("selected");
            }
        }

        private void Awake()
        {
            document = GetComponent<UIDocument>();
            root = document.rootVisualElement[0];
        }

        private void OnEnable()
        {
            inventory.HotbarModified += OnHotbarModified;
            inventory.HotbarSelectionChanged += OnHotbarSelectionChanged;
        }

        private void OnDisable()
        {
            inventory.HotbarModified -= OnHotbarModified;
            inventory.HotbarSelectionChanged -= OnHotbarSelectionChanged;
        }
    }
}
