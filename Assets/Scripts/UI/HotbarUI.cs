using Game.Data.Items;
using Game.Data.Tiles;
using Game.Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.UI
{
    public class HotbarUI : MonoBehaviour
    {
        private UIDocument document;
        private VisualElement root;

        [SerializeField] private Inventory inventory;

        private IUsable[] Hotbar => inventory.Hotbar;

        private void OnHotbarModified()
        {
            for (int i = 0; i < Hotbar.Length; i++)
            {
                Image image = root[i].Q<Image>();
                image.sprite = Hotbar[i]?.Icon;
                image.tintColor = Hotbar[i] is BlockTile block ? block.color : Color.white;
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
