using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChildParticleEmitter : MonoBehaviour
{
	List<ParticleSystem> particleSystems;

	// Use this for initialization
	void Start()
	{
		particleSystems = new List<ParticleSystem>();

		particleSystems.AddRange(GetComponentsInChildren<ParticleSystem>());
	}

	public void Emit(int numParticles)
	{
		foreach(ParticleSystem p in particleSystems)
		{
			p.Emit(numParticles);
		}
	}
}
