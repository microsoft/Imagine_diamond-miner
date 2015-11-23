using UnityEngine;
using System.Collections;

[System.Serializable]
public struct TileType
{
	[Range(0f, 5f)]
	public int minDiamonds;
	[Range(0f, 5f)]
	public int maxDiamonds;

	public int DiamondSpread
	{
		get { return maxDiamonds - minDiamonds; }
	}

	public bool isBomb;

	public int numTiles;
}

[System.Serializable]
public struct Level
{
	public TileType[] tiles;
	public int boardSize;
	public int numDiamonds;
	public int numDigs;
	public int goldScore;
	public int silverScore;
	public int bronzeScore;
	public GameObject levelPrefab;
}

[System.Serializable]
public enum Rank
{
	Gold,
	Silver,
	Bronze,
	Unranked
}
