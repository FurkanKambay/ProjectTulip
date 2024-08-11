using UnityEngine;

namespace Tulip.Data
{
    public interface IUnityComponent
    {
        // ReSharper disable once InconsistentNaming
        public Transform transform { get; }
    }
}
