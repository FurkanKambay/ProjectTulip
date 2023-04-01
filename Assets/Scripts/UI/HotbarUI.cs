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

        private void OnHotbarModified()
        {
            for (int i = 0; i < inventory.Hotbar.Length; i++)
            {
                Image image = root[i].Q<Image>();
                image.sprite = inventory.Hotbar[i]?.Icon;

                if (inventory.Hotbar[i] == null) return;
                image.transform.scale = Vector3.one * inventory.Hotbar[i].IconScale;
                image.tintColor = inventory.Hotbar[i] is BlockTile block ? block.color : Color.white;
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
