using UnityEngine;
using System.Collections;

/// <summary>
/// ScoreParticleHandler - script that responds to particle system collisions.
/// </summary>
public class ScoreParticleHandler : MonoBehaviour
{
	void OnParticleCollision(GameObject other)
	{
		GameplayManager.Instance.IncrementScore();
	}
}
