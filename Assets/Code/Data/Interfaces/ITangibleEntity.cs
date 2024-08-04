using UnityEngine;

namespace Tulip.Data
{
    public interface ITangibleEntity
    {
        public Entity Entity { get; }
        public HealthBase Health { get; }

        public IWorld World { get; }
        public Vector3Int Cell { get; }
        public RectInt Rect { get; }
    }
}
