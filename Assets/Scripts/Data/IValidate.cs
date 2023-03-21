using UnityEngine;

namespace Game.Data
{
    public interface IValidate : ISerializationCallbackReceiver
    {
        void OnValidate();

        void ISerializationCallbackReceiver.OnBeforeSerialize() => this.OnValidate();
        void ISerializationCallbackReceiver.OnAfterDeserialize() {}
    }
}
