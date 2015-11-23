using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileScript : MonoBehaviour
{
	[SerializeField]
	GameObject explosionPrefab = null;

	[SerializeField]
	GameObject hintParticles = null;

	[SerializeField]
	GameObject explodedParticles = null;

	ParticleSystem hintFX;
	ChildParticleEmitter explodeFX;

	[SerializeField]
	float fxOffset = 15f;

	int x, y;

	int numDiamonds;
	bool clickable;

	LevelScript level;

	[SerializeField]
	AudioClip digSound = null;
	AudioSource digSource;

	[SerializeField]
	AudioClip explodeSound = null;
	AudioSource explodeSource;

	[SerializeField]
	AudioClip[] diamondRevealSounds = null;
	List<AudioSource> diamondRevealSources;

	void Start()
	{
		// Setup audio for different diamond amounts
		diamondRevealSources = new List<AudioSource>();
		for(int i = 0; i < diamondRevealSounds.Length; ++i)
		{
			diamondRevealSources.Add(AudioHelper.CreateAudioSource(gameObject, diamondRevealSounds[i]));
		}

		// Setup other audio
		digSource = AudioHelper.CreateAudioSource(gameObject, digSound);
		explodeSource = AudioHelper.CreateAudioSource(gameObject, explodeSound);
	}

	/// <summary>
	/// Initializes the tile with new data.
	/// </summary>
	/// <param name="diamonds">Number of diamonds in the tile.</param>
	/// <param name="xCoord">x-position in the grid for the tile.</param>
	/// <param name="yCoord">y-position in the grid for the tile.</param>
	/// <param name="inLevel">Reference to the level the tile is in.</param>
	public void Initialize(int diamonds, int xCoord, int yCoord, LevelScript inLevel)
	{
		// Attach hint FX
		var hintP = ObjectPooler.Instance.GetPooledObject(hintParticles.name);
		hintP.transform.SetParent(transform);
		hintP.transform.localPosition = new Vector3(0, fxOffset, 0);
		hintP.SetActive(true);
		hintFX = hintP.GetComponent<ParticleSystem>();

		// Attach explosion FX
		var explodeP = ObjectPooler.Instance.GetPooledObject(explodedParticles.name);
		explodeP.transform.SetParent(transform);
		explodeP.transform.localPosition = new Vector3(0, fxOffset, 0);
		explodeP.SetActive(true);
		explodeFX = explodeP.GetComponent<ChildParticleEmitter>();

		// Update variables
		numDiamonds = diamonds;
		clickable = true;
		x = xCoord;
		y = yCoord;
		level = inLevel;
	}

	/// <summary>
	/// Called when the tile is about to be returned to the object pool.
	/// </summary>
	public void OnRecycle()
	{
		// Cleanup grass child object if it's still there
		var grass = GetGrassChild();
		if (grass)
		{
			ObjectPooler.Instance.ReturnPooledObject(grass);
		}

		// Cleanup particle systems
		ObjectPooler.Instance.ReturnPooledObject(hintFX.gameObject);
		ObjectPooler.Instance.ReturnPooledObject(explodeFX.gameObject);
	}

	void Update()
	{
		// Detect clicks
		if (Input.GetMouseButtonUp(0))
		{
			// Attempt to raycast mouse click to this tile
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit info;
			if (GetComponent<Collider>().Raycast(ray, out info, 1000f))
			{
				// Player clicked on this tile
				OnClicked();
			}
		}
	}

	/// <summary>
	/// Called when the player clicks on this tile.
	/// </summary>
	void OnClicked()
	{
		// *** Add your source code here ***
	}

	/// <summary>
	/// Plays the hint animation on this tile.
	/// </summary>
	public void PlayAnimation()
	{
		if (clickable)
		{
			// Change intensity based on number of diamonds
			hintFX.emissionRate = numDiamonds > 0 ? numDiamonds * numDiamonds * 3 : 0;
			hintFX.Play();
		}
	}

	/// <summary>
	/// Explodes the tile after a delay.
	/// </summary>
	/// <param name="delay">The delay before exploding, in seconds.</param>
	/// <returns>true if the tile exploded, false if it is not able to explode anymore.</returns>
	public bool Explode(float delay)
	{
		if (clickable)
		{
			clickable = false;
			Invoke("DoExplode", delay);
			return true;
		}

		return false;
	}

	/// <summary>
	/// Explodes the tile, spawning effects as necessary.
	/// </summary>
	void DoExplode()
	{
		// Remove grass covering
		ObjectPooler.Instance.ReturnPooledObject(GetGrassChild());

		// Play explosion FX
		explodeFX.Emit(numDiamonds * numDiamonds);
		explodeSource.Play();

		// Chain reaction if this tile was a bomb
		if (numDiamonds < 0)
		{
			SpawnExplosion();
			level.OnTileActivated(x, y);
		}
		else
		{
			// Remove any diamonds on this tile from play
			GameplayManager.Instance.RemoveDiamonds(numDiamonds);
		}
	}

	/// <summary>
	/// Spawn an overhead explosion animation.
	/// </summary>
	void SpawnExplosion()
	{
		// Grab an explosion prefab from the object pool
		var explosion = ObjectPooler.Instance.GetPooledObject(explosionPrefab.name);
		// Move explosion to the tile
		explosion.transform.SetParent(transform);
		explosion.transform.localPosition = new Vector3(0, fxOffset, 0);
		explosion.SetActive(true);
		// Play explosion
		explosion.GetComponent<ParticleSystem>().Play();
	}

	/// <summary>
	/// Gets the child grass covering for this tile, if it exists.
	/// </summary>
	/// <returns>The grass covering for this tile, or null if it no longer exists.</returns>
	GameObject GetGrassChild()
	{
		for (int i = 0; i < transform.childCount; ++i)
		{
			var c = transform.GetChild(i);
			if (c.CompareTag("Tile"))
			{
				return c.gameObject;
			}
		}

		return null;
	}
}
