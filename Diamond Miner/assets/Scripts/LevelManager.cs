using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour
{
	public static LevelManager Instance;

	public Level[] levels = new Level[0];

	int currentIndex;
	GameObject currentLevel;
	
	void Awake()
	{
		Instance = this;
	}

	/// <summary>
	/// Loads a level and returns the number of diamonds in the level.
	/// </summary>
	/// <param name="index"></param>
	/// <returns>The number of diamonds in the level.</returns>
	public int StartLevel(int index)
	{
		currentIndex = Mathf.Clamp(index - 1, 0, levels.Length);

		if (currentLevel != null)
		{
			// Return objects to pool instead of destroying
			currentLevel.GetComponent<LevelScript>().RecycleTiles();
			Destroy(currentLevel);
		}

		// Create the new level
		currentLevel = Instantiate(levels[currentIndex].levelPrefab) as GameObject;
		currentLevel.GetComponent<LevelScript>().GenerateAndDisplay(levels[currentIndex]);

		return levels[currentIndex].numDiamonds;
	}

	/// <summary>
	/// Gets the number of digs allowed for the current level.
	/// </summary>
	/// <returns>The number of digs allowed for the current level.</returns>
	public int GetDigsForCurrentLevel()
	{
		return levels[currentIndex].numDigs;
	}

	/// <summary>
	/// Gets the rank associated with a specific score.
	/// </summary>
	/// <param name="score"></param>
	/// <returns></returns>
	public Rank GetRankForScore(int score)
	{
		Rank rank = Rank.Unranked;
		if (score >= levels[currentIndex].goldScore)
		{
			rank = Rank.Gold;
		}
		else if (score >= levels[currentIndex].silverScore)
		{
			rank = Rank.Silver;
		}
		else if (score >= levels[currentIndex].bronzeScore)
		{
			rank = Rank.Bronze;
		}

		return rank;
	}

	public int GetScoreForNextRank(int score)
	{
		Rank currentRank = GetRankForScore(score);
		switch (currentRank)
		{
			case Rank.Unranked:
				return levels[currentIndex].bronzeScore;
			case Rank.Bronze:
				return levels[currentIndex].silverScore;
			case Rank.Silver:
				return levels[currentIndex].goldScore;
			default:
				return -1;
		}
	}

	/// <summary>
	/// Plays the hint animation for the current level.
	/// </summary>
	public void AnimateLevel()
	{
		if (currentLevel)
		{
			currentLevel.GetComponent<LevelScript>().Animate();
		}
	}
}
