using UnityEngine;
using System.Collections;

/// <summary>
/// ScoreParticleEmitter - script that allows re-use of a particle system to emit
/// bursts of particles in specific locations.
/// </summary>
public class ScoreParticleEmitter : MonoBehaviour
{
	public static ScoreParticleEmitter Instance { get; private set; }

	ParticleSystem myParticleSystem;

	void Awake()
	{
		Instance = this;
		myParticleSystem = GetComponent<ParticleSystem>();
	}

	/// <summary>
	/// Moves the particle system to a location and emits a number of particles from there.
	/// </summary>
	/// <param name="position">The location from which to emit.</param>
	/// <param name="numParticles">The number of particles to emit.</param>
	public void EmitParticles(Vector3 position, int numParticles)
	{
		transform.position = position + new Vector3(0, 0, -15);
		myParticleSystem.Emit(numParticles);
	}
}
