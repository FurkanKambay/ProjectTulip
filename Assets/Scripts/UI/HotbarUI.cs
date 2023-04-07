using Game.Data.Interfaces;
using Game.Data.Tiles;
using Game.Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
    public class HotbarUI : MonoBehaviour
    {
        [SerializeField] private Inventory inventory;

        private UIDocument document;
        private VisualElement root;

        private void OnHotbarModified()
        {
            IItem[] items = inventory.Items;
            for (int i = 0; i < items.Length; i++)
            {
                Image image = root[i].Q<Image>();
                image.sprite = items[i]?.Icon;

                if (items[i] == null) return;
                image.transform.scale = Vector3.one * items[i].IconScale;
                image.tintColor = items[i] is BlockTile block ? block.color : Color.white;
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
