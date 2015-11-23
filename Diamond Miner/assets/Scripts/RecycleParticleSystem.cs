using UnityEngine;
using System.Collections;

/// <summary>
/// RecycleParticleSystem - script that returns an object to the ObjectPooler when an attached
/// ParticleSystem finishes playing.
/// </summary>
public class RecycleParticleSystem : MonoBehaviour
{
	ParticleSystem ps;

	void Start()
	{
		ps = GetComponent<ParticleSystem>();
		if (!ps)
		{
			// Return object to pool if no particle system is present
			Recycle();
		}
	}
	
	void Update()
	{
		if (!ps.IsAlive())
		{
			// Return object to pool if particle system is finished emitting
			Recycle();
		}
	}

	/// <summary>
	/// Returns the object to the ObjectPooler.
	/// </summary>
	void Recycle()
	{
		ObjectPooler.Instance.ReturnPooledObject(gameObject);
	}
}
