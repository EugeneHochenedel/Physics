using UnityEngine;

namespace ParticleProperties
{
	public interface IParticles
	{
		Vector3 Position { get; set; }
		Vector3 Velocity { get; set; }
		Vector3 Force { get; set; }
		float Mass { get; set; }
	}
}