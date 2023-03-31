using UnityEngine;

namespace Game.Data.Interfaces
{
    public interface IItem
    {
        Sprite Icon { get; }
        float IconScale { get; }
    }
}
