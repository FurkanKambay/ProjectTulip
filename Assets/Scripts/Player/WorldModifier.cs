using Game.Data.Items;
using UnityEngine;

namespace Game.Player
{
    public class WorldModifier : MonoBehaviour
    {
        private float timeSinceLastUse;

        private void Update()
        {
            IUsable item = Inventory.Instance.HotbarSelected;

            timeSinceLastUse += Time.deltaTime;
            if (timeSinceLastUse < item.UseTime) return;

            if (!Input.Actions.Player.Fire.IsPressed()) return;
            timeSinceLastUse = 0;

            Vector2 mouse = Input.Instance.MouseWorldPoint;
            item.Use(World.Instance.Tilemap.WorldToCell(mouse));
        }
    }
}
