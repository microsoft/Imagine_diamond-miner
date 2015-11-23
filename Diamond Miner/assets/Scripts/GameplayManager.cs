using UnityEngine;
using System.Collections;

/// <summary>
/// The class that controls gameplay flow.
/// </summary>
public class GameplayManager : MonoBehaviour
{
	public static GameplayManager Instance { get; private set; }
	
	enum GameState
	{
		InGame,		// Player can start shooting with the left mouse button
		LevelFailed, // Player needs to restart the level
		LevelCompleted, // Player can move to the next level
		GameOver,	// Game ended, player input is blocked
	};
	GameState state = GameState.GameOver;

	[SerializeField]
	int hints = 4;

	// State management
	int currentLevel;
	int currentScore;
	int digsUsed;
	int hintsRemaining;
	int hintsUsedThisLevel;
	int diamondsRemaining;

	float hintCooldown = 3f;
	float hintTimer = 0f;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Instance = this;
	}

	/// <summary>
	/// Called when this behaviour gets initialized.
	/// See Unity docs for more information.
	/// </summary>
	void Start()
	{
		currentLevel = 1;
		currentScore = 0;
		diamondsRemaining = 0;
		digsUsed = 0;
		hintsRemaining = hints;
		hintsUsedThisLevel = 0;

		diamondsRemaining = LevelManager.Instance.StartLevel(currentLevel);

		UIManager.Instance.UpdateHUD(currentLevel, currentScore, LevelManager.Instance.GetDigsForCurrentLevel() - digsUsed, LevelManager.Instance.GetRankForScore(currentScore), hintsRemaining, LevelManager.Instance.GetScoreForNextRank(currentScore));
		UIManager.Instance.ShowHUD(false);
		UIManager.Instance.ShowScreen("Tutorial");
	}

	/// <summary>
	/// Called once per frame.
	/// See Unity docs for more information.
	/// </summary>
	void Update()
	{
		// Update hint cooldown
		if (hintTimer > 0f)
		{
			hintTimer = Mathf.Max(0f, hintTimer - Time.deltaTime);
		}
	}

	/// <summary>
	/// Logic that runs when we are requested to start gameplay.
	/// </summary>
	public void OnStartGame()
	{
		// Show the HUD
		UIManager.Instance.ShowHUD(true);

		// Hide all UI screens
		UIManager.Instance.ShowScreen("");

		// Invoke calls a function in the future. In this case, we are doing this to prevent
		// the level from receiving the mouse events from dismissing the screen.
		Invoke("StartGame", 0.1F);
	}

	/// <summary>
	/// Sets our game state to in-game so the player can start interacting with the level.
	/// </summary>
	void StartGame()
	{
		state = GameState.InGame;
		LevelManager.Instance.AnimateLevel();
	}

	/// <summary>
	/// Logic that runs when we are requested to restart the current level.
	/// </summary>
	public void OnRetryLevel()
	{		
		// Reload current level
		diamondsRemaining = LevelManager.Instance.StartLevel(currentLevel);
		LevelManager.Instance.AnimateLevel();

		// Start gameplay and update UI
		UIManager.Instance.ShowScreen("");

		currentScore = 0;
		digsUsed = 0;
		hintsRemaining += hintsUsedThisLevel;
		hintsUsedThisLevel = 0;
		UIManager.Instance.UpdateHUD(currentLevel, currentScore, LevelManager.Instance.GetDigsForCurrentLevel() - digsUsed, LevelManager.Instance.GetRankForScore(currentScore), hintsRemaining, LevelManager.Instance.GetScoreForNextRank(currentScore));
		state = GameState.InGame;
	}

	/// <summary>
	/// Logic that runs when we are requested to move to the next level.
	/// </summary>
	public void OnNextLevel()
	{
		// Advance to the next level
		currentLevel = (currentLevel == LevelManager.Instance.levels.Length) ? 1 : currentLevel + 1;

		hintsUsedThisLevel = 0;

		// Re-use retry level logic since it resets the level for us
		OnRetryLevel();
	}

	/// <summary>
	/// Raises the restart event.
	/// </summary>
	public void OnRestart()
	{
		// Reload the current scene
		Application.LoadLevel(Application.loadedLevel);
	}

	/// <summary>
	/// Logic that runs when the level tells us it has been completed.
	/// </summary>
	public void OnLevelCompleted()
	{
		state = GameState.LevelCompleted;

		// Show the Game Complete screen if we have finished all the levels
		Invoke("CompleteLevel", 1f);
	}

	/// <summary>
	/// Logic that runs when we have failed a level.
	/// </summary>
	public void OnLevelFailed()
	{
		state = GameState.LevelFailed;
		Invoke("FailLevel", 1f);
	}

	/// <summary>
	/// Shows a game completion or level completion screen, depending on which level the player
	/// just completed.
	/// </summary>
	void CompleteLevel()
	{
		//Last level, show Game Complete
		if (currentLevel == LevelManager.Instance.levels.Length)
		{
			UIManager.Instance.ShowScreen("Game Complete");
		}
		else
		{
			UIManager.Instance.ShowScreen("Level Complete");
		}
	}

	/// <summary>
	/// Shows the Game Over screen.
	/// </summary>
	void FailLevel()
	{
		UIManager.Instance.ShowScreen("Game Over");
	}

	/// <summary>
	/// Determines whether the player can launch to start gameplay.
	/// </summary>
	/// <returns><c>true</c> if the player can shoot; otherwise, <c>false</c>.</returns>
	public bool CanUpdateGame()
	{
		// The player can only shoot if we're actually in-game.
		return state == GameState.InGame;
	}

	/// <summary>
	/// This is called when diamonds have been collected by clicking on a tile with diamonds.
	/// </summary>
	/// <param name="diamonds"></param>
	public void AddDiamondsToScore(int diamonds)
	{
		// Update remaining diamonds and score
		diamondsRemaining -= diamonds;
		currentScore += diamonds;

		UIManager.Instance.UpdateHUD(currentLevel, currentScore, LevelManager.Instance.GetDigsForCurrentLevel() - digsUsed, LevelManager.Instance.GetRankForScore(currentScore), hintsRemaining, LevelManager.Instance.GetScoreForNextRank(currentScore));

		// End game if there are no more diamonds
		if (diamondsRemaining <= 0)
		{
			FinishLevel();
		}
	}

	/// <summary>
	/// This is called when a tile with diamonds explodes without being collected.
	/// </summary>
	/// <param name="diamonds"></param>
	public void RemoveDiamonds(int diamonds)
	{
		diamondsRemaining -= diamonds;
		// End game if there are no more diamonds
		if (diamondsRemaining <= 0)
		{
			FinishLevel();
		}
	}

	/// <summary>
	/// This is called when the player digs up a tile.
	/// </summary>
	public void UseDig()
	{
		++digsUsed;
		UIManager.Instance.UpdateHUD(currentLevel, currentScore, LevelManager.Instance.GetDigsForCurrentLevel() - digsUsed, LevelManager.Instance.GetRankForScore(currentScore), hintsRemaining, LevelManager.Instance.GetScoreForNextRank(currentScore));

		if (digsUsed >= LevelManager.Instance.GetDigsForCurrentLevel())
		{
			FinishLevel();
		}
	}

	/// <summary>
	/// This is called when the level has been finished in one way or another.
	/// </summary>
	public void FinishLevel()
	{
		// Level completed if player achieved a rank of Bronze or higher
		if (LevelManager.Instance.GetRankForScore(currentScore) != Rank.Unranked)
		{
			OnLevelCompleted();
		}
		else
		{
			OnLevelFailed();
		}
	}

	/// <summary>
	/// Called when the hint button is pressed. This button has a cooldown where it will not do anything,
	/// to prevent multiple presses.
	/// </summary>
	public void OnHintPressed()
	{
		// Hints only allowed if there are some remaining and it has been some time since the last hint. 
		if (state == GameState.InGame && hintsRemaining > 0 && hintTimer <= 0f)
		{
			// Play hint animation
			LevelManager.Instance.AnimateLevel();

			// Update hints used
			--hintsRemaining;
			++hintsUsedThisLevel;
			UIManager.Instance.UpdateHUD(currentLevel, currentScore, LevelManager.Instance.GetDigsForCurrentLevel() - digsUsed, LevelManager.Instance.GetRankForScore(currentScore), hintsRemaining, LevelManager.Instance.GetScoreForNextRank(currentScore));
			hintTimer = hintCooldown;
		}
	}

	/// <summary>
	/// Called when the retry button is pressed.
	/// </summary>
	public void OnRetryPressed()
	{
		// Button resets only in-game or when the level was failed
		if (state == GameState.InGame || state == GameState.LevelFailed)
		{
			OnRetryLevel();
		}
	}

	/// <summary>
	/// This increments the score by 1.
	/// </summary>
	public void IncrementScore()
	{
		diamondsRemaining--;
		currentScore++;
		UIManager.Instance.UpdateHUD(currentLevel, currentScore, LevelManager.Instance.GetDigsForCurrentLevel() - digsUsed, LevelManager.Instance.GetRankForScore(currentScore), hintsRemaining, LevelManager.Instance.GetScoreForNextRank(currentScore));

		// End game if there are no diamonds remaining
		if (diamondsRemaining <= 0)
		{
			FinishLevel();
		}
	}

	/// <summary>
	/// This is called from the LanguageMenu when languages are changed.
	/// </summary>
	public void OnLanguageChanged()
	{
		UIManager.Instance.OnLanguageChanged();
		UIManager.Instance.UpdateHUD(currentLevel, currentScore, LevelManager.Instance.GetDigsForCurrentLevel() - digsUsed, LevelManager.Instance.GetRankForScore(currentScore), hintsRemaining, LevelManager.Instance.GetScoreForNextRank(currentScore));
	}
}