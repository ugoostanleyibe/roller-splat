using UnityEngine.SceneManagement;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
	private GroundPieceController[] allGroundPieces;

	public static GameManager singleton;

	private void Awake()
	{
		if (singleton == null)
		{
			singleton = this;
		}
		else if (singleton != this)
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		SetUpNewLevel();
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	public void CheckCompletion()
	{
		if (allGroundPieces.All(piece => piece.isColoured))
		{
			FindObjectOfType<BallController>().Freeze();
			Invoke(nameof(LoadNextLevel), 1.28f);
		}
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		SetUpNewLevel();
	}

	private void LoadNextLevel()
	{
		SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
	}

	private void SetUpNewLevel()
	{
		allGroundPieces = FindObjectsOfType<GroundPieceController>();
	}
}
