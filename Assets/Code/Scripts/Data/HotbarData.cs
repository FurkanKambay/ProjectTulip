using System;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "Hotbar Data", menuName = "Data/Hotbar")]
    public class HotbarData : ScriptableObject, IValidate
    {
        public ScriptableObject[] hotbar;

        public void OnValidate()
        {
            hotbar ??= new ScriptableObject[9];
            if (hotbar.Length != 9)
                Array.Resize(ref hotbar, 9);
        }
    }
}
