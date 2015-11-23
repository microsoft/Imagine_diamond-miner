using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// User interface manager.
/// </summary>
public class UIManager : MonoBehaviour
{
	public static UIManager Instance;

	// Fields to set in the Unity editor
	public GameObject[] screens;
	public GameObject hud;
	public Text levelText;
	public Text scoreText;
	public Text rankText;
	public Text digsText;
	public Text hintsText;
	public Text nextRankText;

	[SerializeField]
	AudioClip buttonSound = null;
	AudioSource buttonSource;

	void Awake()
	{
		Instance = this;
		
	}

	void Start()
	{
		buttonSource = AudioHelper.CreateAudioSource(gameObject, buttonSound);

		var colliderPos = new Vector3(0.5f, 1f, 80f);
		colliderPos = Camera.main.ViewportToWorldPoint(colliderPos);
	}

	/// <summary>
	/// Show a screen.
	/// </summary>
	/// <param name="name">The name of the screen.</param>
	public void ShowScreen(string name)
	{
		// Show the screen with the given name and hide everything else
		foreach (GameObject screen in screens)
		{
			screen.SetActive(screen.name == name);
		}
	}

	/// <summary>
	/// Shows/hides the HUD.
	/// </summary>
	/// <param name="show">Do we show the HUD?</param>
	public void ShowHUD(bool show)
	{
		hud.SetActive(show);
	}

	/// <summary>
	/// Updates the HUD elements.
	/// </summary>
	/// <param name="level">The current level.</param>
	/// <param name="score">The player's score.</param>
	/// <param name="digs">The number of digs remaining.</param>
	/// <param name="rank">The player's current rank.</param>
	/// <param name="hints">The number of hints remaining.</param>
	/// <param name="nextRankScore">The score needed for the next rank.</param>
	public void UpdateHUD(int level, int score, int digs, Rank rank, int hints, int nextRankScore)
	{
		ShowLevel(level);
		ShowScore(score);
		ShowDigs(digs);
		ShowRank(rank);
		ShowHints(hints);
		ShowNextRank(nextRankScore);
	}

	/// <summary>
	/// Updates the level text on the HUD.
	/// </summary>
	/// <param name="level">The level number.</param>
	void ShowLevel(int level)
	{
		levelText.text = string.Format(LocalizationManager.Instance.GetString("HUD Level"), level.ToString());
	}

	/// <summary>
	/// Updates the score text on the HUD.
	/// </summary>
	/// <param name="score">Score.</param>
	void ShowScore(int score)
	{
		scoreText.text = string.Format(LocalizationManager.Instance.GetString("HUD Score"), score.ToString());
	}

	/// <summary>
	/// Updates the rank text on the HUD.
	/// </summary>
	/// <param name="rank">Rank.</param>
	void ShowRank(Rank rank)
	{
		string rankKey = "Rank " + rank.ToString();
		rankText.text = string.Format(LocalizationManager.Instance.GetString("HUD Rank"), LocalizationManager.Instance.GetString(rankKey));
	}

	/// <summary>
	/// Updates the dig text on the HUD.
	/// </summary>
	/// <param name="digs">Digs.</param>
	void ShowDigs(int digs)
	{
		digsText.text =  string.Format(LocalizationManager.Instance.GetString("HUD Digs"), digs.ToString());
	}

	/// <summary>
	/// Updates the hint text on the HUD.
	/// </summary>
	/// <param name="hints">Hints.</param>
	void ShowHints(int hints)
	{
		hintsText.text = string.Format(LocalizationManager.Instance.GetString("Button Hints"), hints.ToString());
	}

	/// <summary>
	/// Updates the next rank score text on the HUD.
	/// </summary>
	/// <param name="nextRankScore"></param>
	void ShowNextRank(int nextRankScore)
	{
		if (nextRankScore >= 0)
		{
			nextRankText.text = string.Format(LocalizationManager.Instance.GetString("HUD Next Rank"), nextRankScore.ToString());
		}
		else
		{
			nextRankText.text = "";
		}
	}

	/// <summary>
	/// Called when a button is pressed.
	/// </summary>
	public void OnButton()
	{
		if (buttonSource)
		{
			// Play audio
			buttonSource.Play();
		}
	}

	/// <summary>
	/// Call this when a new language is selected.
	/// </summary>
	public void OnLanguageChanged()
	{
		// Refresh all the static text fields.
		var objs = FindObjectsOfType<StaticTextManager>();
		foreach (StaticTextManager staticText in objs)
		{
			staticText.OnLanguageChanged();
		}
	}
}