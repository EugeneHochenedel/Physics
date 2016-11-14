using UnityEngine;

namespace Boid
{
    public interface IBoid
    {
        Vector3 Velocity { get; set; }
        Vector3 Position { get; set; }
        float Mass { get; set; }
    }
}
