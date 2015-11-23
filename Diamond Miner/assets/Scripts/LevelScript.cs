using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelScript : MonoBehaviour
{
	[SerializeField]
	GameObject emptyTilePrefab = null;
	[SerializeField]
	GameObject bombTilePrefab = null;
	[SerializeField]
	GameObject diamondTilePrefab = null;

	[SerializeField]
	GameObject[] grassPrefabs = null;

	float tileOverlap = 2f;

	Level level;
	int[,] board;
	GameObject[,] boardObjects;

	[SerializeField]
	AudioClip bombSound = null;
	AudioSource bombSource;

	[SerializeField]
	AudioClip hintSound = null;
	AudioSource hintSource;

	int diamondsRemaining = 0;

	void Awake()
	{
		bombSource = AudioHelper.CreateAudioSource(gameObject, bombSound);
		hintSource = AudioHelper.CreateAudioSource(gameObject, hintSound);
	}

	// Update is called once per frame
	void Update()
	{
	}

	/// <summary>
	/// Return pooled tile objects to the ObjectPooler.
	/// </summary>
	public void RecycleTiles()
	{
		foreach (var o in boardObjects)
		{
			o.GetComponent<TileScript>().OnRecycle();

			ObjectPooler.Instance.ReturnPooledObject(o);
		}
	}

	/// <summary>
	/// Generate and show the current level.
	/// </summary>
	/// <param name="levelToGenerate">The level to generate.</param>
	public void GenerateAndDisplay(Level levelToGenerate)
	{
		level = levelToGenerate;
		// Don't display a level if it didn't generate
		if (Generate())
		{
			Display();
		}
	}

	/// <summary>
	/// Randomly generates the level.
	/// </summary>
	/// <returns>true if the level generated correctly, false otherwise.</returns>
	bool Generate()
	{
		// Validate the level parameters to make sure it's possible to generate
		if (!Validate())
		{
			return false;
		}

		var diamondsToPlace = level.numDiamonds;
		diamondsRemaining = diamondsToPlace;

		int numTiles = level.boardSize * level.boardSize;

		// Generate underlying 2D array
		board = new int[level.boardSize, level.boardSize];

		// Make an array of tiles to shuffle
		TileType[] shuffledTiles = GenerateTileInformation(numTiles);

		// Shuffle tiles
		ShuffleArray<TileType>(shuffledTiles);

		// Bump each board tile up to its tile minimum
		diamondsToPlace -= PopulateTilesToMinimum(shuffledTiles);
		
		// Place remaining diamonds
		PlaceSurplusDiamonds(shuffledTiles, diamondsToPlace);

		// We successfully generated if we got this far
		return true;
	}

	/// <summary>
	/// Generates a list of tiles based on the level information.
	/// </summary>
	/// <param name="numTiles">The number of tiles to generate.</param>
	/// <returns>An array of tile information.</returns>
	TileType[] GenerateTileInformation(int numTiles)
	{
		TileType[] tiles = new TileType[numTiles];
		int index = 0;

		// Make a list of expected tile types
		for (int i = 0; i < level.tiles.Length; ++i)
		{
			for (int j = 0; j < level.tiles[i].numTiles; ++j)
			{
				bool isBomb = level.tiles[i].isBomb;
				tiles[index].isBomb = isBomb;

				// Bomb overrides everything
				if (isBomb)
				{
					tiles[index].minDiamonds = 0;
					tiles[index].maxDiamonds = 0;
				}
				else
				{
					tiles[index].minDiamonds = level.tiles[i].minDiamonds;
					tiles[index].maxDiamonds = level.tiles[i].maxDiamonds;
				}
				++index;
			}
		}

		return tiles;
	}

	/// <summary>
	/// Shuffles an array in-place.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="arr">The array to shuffle.</param>
	void ShuffleArray<T>(T[] arr)
	{
		// *** Add your source code here ***
	}
	
	/// <summary>
	/// Populates each tile with the minimum required diamonds.
	/// </summary>
	/// <param name="tiles">The list of tile information to use.</param>
	/// <returns>The number of diamonds placed.</returns>
	int PopulateTilesToMinimum(TileType[] tiles)
	{
		int index = 0;
		int placedDiamonds = 0;
		for (int x = 0; x < board.GetLength(0); ++x)
		{
			for (int y = 0; y < board.GetLength(1); ++y)
			{
				// Bombs are stored as -1
				if (tiles[index].isBomb)
				{
					board[x, y] = -1;
				}
				else
				{
					// Populate the tile with the minimum allowed diamonds
					board[x, y] = tiles[index].minDiamonds;
					placedDiamonds += tiles[index].minDiamonds;
				}
				++index;
			}
		}

		return placedDiamonds;
	}

	/// <summary>
	/// Places extra diamonds above the minimum required amount.
	/// </summary>
	/// <param name="tiles">The tile information to use for placement.</param>
	/// <param name="diamondsToPlace">The number of extra diamonds to place.</param>
	void PlaceSurplusDiamonds(TileType[] tiles, int diamondsToPlace)
	{
		// *** Add your source code here ***
	}

	/// <summary>
	/// Validates that the level information can be used to make a valid level.
	/// </summary>
	/// <returns>true if the level information is usable, false otherwise.</returns>
	bool Validate()
	{
		var numTiles = level.boardSize * level.boardSize;
		var actualTiles = 0;
		var actualMinDiamonds = 0;
		var actualMaxDiamonds = 0;

		//Calculate the number of diamonds required in the level
		for (int i = 0; i < level.tiles.Length; ++i)
		{
			actualTiles += level.tiles[i].numTiles;
			if (!level.tiles[i].isBomb)
			{
				actualMinDiamonds += (level.tiles[i].minDiamonds * level.tiles[i].numTiles);
				actualMaxDiamonds += (level.tiles[i].maxDiamonds * level.tiles[i].numTiles);
			}
		}

		bool goodToGo = true;
		if (actualTiles > numTiles)
		{
			goodToGo = false;
			Debug.LogWarning("Too many tiles specified for level!");
		}
		if (actualTiles < numTiles)
		{
			goodToGo = false;
			Debug.LogWarning("Not enough tiles specified for level!");
		}
		if (actualMinDiamonds > level.numDiamonds)
		{
			goodToGo = false;
			Debug.LogWarning("Too many diamonds required to spawn in tiles!");
		}
		if (actualMaxDiamonds < level.numDiamonds)
		{
			goodToGo = false;
			Debug.LogWarning("Not enough diamonds will spawn in the level!");
		}

		return goodToGo;
	}

	/// <summary>
	/// Generates objects to represent the underlying level data and shows them in the world.
	/// </summary>
	void Display()
	{
		var xLen = board.GetLength(0);
		var yLen = board.GetLength(1);
		boardObjects = new GameObject[xLen, yLen];

		var tileRender = emptyTilePrefab.GetComponent<Renderer>();
		var tileBounds = tileRender.bounds;
		for (int x = 0; x < xLen; ++x)
		{
			for (int y = 0; y < yLen; ++y)
			{
				// Calculate tile position based on (x,y) values
				var xOffset = -0.5f * (xLen - 1) + x;
				var yOffset = -0.5f * (yLen - 1) + y;
				xOffset *= (tileBounds.size.x - tileOverlap);
				yOffset *= (tileBounds.size.y - tileOverlap);

				Vector3 tilePos = new Vector3(xOffset, yOffset, 0f);

				GameObject tileObj = null;

				// Populate board tile with correct type
				switch(board[x,y])
				{
					case -1:
						tileObj = ObjectPooler.Instance.GetPooledObject(bombTilePrefab.name);
						break;
					case 0:
						tileObj = ObjectPooler.Instance.GetPooledObject(emptyTilePrefab.name);
						break;
					default:
						tileObj = ObjectPooler.Instance.GetPooledObject(diamondTilePrefab.name);
						break;
				}

				// Put a random grass overlay on top
				var grass = ObjectPooler.Instance.GetPooledObject(grassPrefabs[Random.Range(0, grassPrefabs.Length)].name);
				grass.transform.SetParent(tileObj.transform);
				grass.transform.localPosition = Vector3.zero;
				grass.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
				grass.SetActive(true);

				// Move the tile to the correct position
				tileObj.transform.SetParent(gameObject.transform);
				tileObj.transform.localPosition = tilePos;
				tileObj.SetActive(true);

				// Setup tile with correct information
				tileObj.GetComponent<TileScript>().Initialize(board[x, y], x, y, this);
				boardObjects[x, y] = tileObj;
			}
		}
	}

	/// <summary>
	/// Play the hint animation for the level.
	/// </summary>
	public void Animate()
	{
		hintSource.Play();

		for(int x = 0; x < boardObjects.GetLength(0); ++x)
		{
			for (int y = 0; y < boardObjects.GetLength(1); ++y)
			{
				// Run as coroutine to allow delay
				StartCoroutine(AnimateTile(x, y));
			}
		}
	}

	/// <summary>
	/// Plays the hint animation for a specific tile.
	/// </summary>
	/// <param name="x">The x-coordinate of the tile.</param>
	/// <param name="y">The y-coordinate of the tile.</param>
	/// <returns></returns>
	IEnumerator AnimateTile(int x, int y)
	{
		// Delay animation based on location for board sweeping effect
		yield return new WaitForSeconds(0.05f * (x + y));
		boardObjects[x, y].GetComponent<TileScript>().PlayAnimation();
	}

	/// <summary>
	/// Performs logic when a tile is clicked or activated that affects other tiles.
	/// </summary>
	/// <param name="x">The x-coordinate of the tile.</param>
	/// <param name="y">The y-coordinate of the tile.</param>
	public void OnTileActivated(int x, int y)
	{
		// *** Add your source code here ***
		{
			// Play hint animations for adjacent tiles
			if (x > 0)
			{
				boardObjects[x - 1, y].GetComponent<TileScript>().PlayAnimation();
			}
			if (x + 1 < boardObjects.GetLength(0))
			{
				boardObjects[x + 1, y].GetComponent<TileScript>().PlayAnimation();
			}
			if (y > 0)
			{
				boardObjects[x, y - 1].GetComponent<TileScript>().PlayAnimation();
			}
			if (y + 1 < boardObjects.GetLength(1))
			{
				boardObjects[x, y + 1].GetComponent<TileScript>().PlayAnimation();
			}
		}
	}
}
