using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	const int ENEMY_NUM = 3;

	const float ENEMY_SPAWN_POS_Y	= 12;
	const float PLAYER_SPAWN_POS_Y	= 17;

	const int SHIFT_PLAYER_POS_Y	= 3;

	const int LEVEL_EASY	= 1;
	const int LEVEL_NORMAL	= 2;
	const int LEVEL_HARD	= 3;

	const float ENEMY_SPEED_EASY	= 4f;
	const float ENEMY_SPEED_MEDIUM	= 6f;
    const float ENEMY_SPEED_HARD	= 8f;

    public const int MODE_ATTACK	= 1;
	public const int MODE_DEFENSE	= 2;

	public const int SCORE_GAIN_TYPE_ADVANTAGE = 3;
	public const int SCORE_GAIN_TYPE_SAME = 1;

	public const int SEC_COUNT_BEFORE_START = 3;

	public GameObject[] enemiesPrefabs;
	public GameObject[] playerPrefabs;
	public GameObject indicator;
	public GameObject enemySpawnIndicator;
	public GameObject enemyWall;
	public GameObject ingameInstruction;
	public GameObject playerHPBar;
	public GameObject enemyHPBar;
    public GameObject firework;

    public TextMeshProUGUI scoreText;

    private GameMenuManager gameMenuManager;

    private Color colorEasy = new Color(0.74f, 0.7f, 0.05f);
    private Color colorNormal = new Color(1.0f, 0.64f, 0.0f);
    private Color colorHard = new Color(0.52f, 0.0f, 0.73f);

    private Vector3 fireworkPos = new Vector3(0, -6, -13);

    private Vector3[] enemySpawnPos =
	{
		new Vector3(-6, 0, ENEMY_SPAWN_POS_Y),
		new Vector3(0, 0, ENEMY_SPAWN_POS_Y),
		new Vector3(6, 0, ENEMY_SPAWN_POS_Y)
	};

	private Vector3[] playerSpawnPos =
	{
		new Vector3(-6, 0, -PLAYER_SPAWN_POS_Y),
		new Vector3(0, 0, -PLAYER_SPAWN_POS_Y),
		new Vector3(6, 0, -PLAYER_SPAWN_POS_Y)
	};

    private int curPlayerSpawnPosIndex = 0;
	private bool playerSpawnable = false;
	private bool isGameOver = true;
	private int difficulty;
	private int mode;
    private int score = 0;

	public bool GameOver {
		get {
			return isGameOver; 
		}
		set
		{
			isGameOver = value;
		}
	}

    public int GameMode {  get { return mode; } }

	public int Score {
		get { return score; }
		set {  
			score = Math.Max(0, value);
		}
	}

    void Start()
	{
		SetIndicator(curPlayerSpawnPosIndex);
		gameMenuManager = GameObject.Find("GameMenuManager").GetComponent<GameMenuManager>();
    }

	void Update()
	{

		DisplayScore();
        if (!isGameOver)
		{
			UpdateGameByMode();
            HandlelayerInput();
        }
	}

	public void DisplayScore()
	{
		scoreText.text = "Score: " + score;
    }

	void UpdateGameByMode()
	{
		int enemyNum = GameObject.FindGameObjectsWithTag("Enemy").Length;
        switch (mode)
		{
			case MODE_ATTACK:
				if (enemyNum <= 0)
				{
					SpawnEnemy();
				}
                break;
			case MODE_DEFENSE:
				if (enemyNum <= 0)
				{
					SpawnEnemy();
					ShiftPlayerPos();
				}
                break;
			default:
				return;
		}
	}

    void HandlelayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SpawnElement(Element.TYPE_FIRE);
            SetIndicator(curPlayerSpawnPosIndex);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SpawnElement(Element.TYPE_WATER);
            SetIndicator(curPlayerSpawnPosIndex);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnElement(Element.TYPE_WOOD);
            SetIndicator(curPlayerSpawnPosIndex);
        }
    }

    void SpawnElement(int type)
	{
		if (playerSpawnable)
		{
			Instantiate(
				playerPrefabs[type],
				playerSpawnPos[curPlayerSpawnPosIndex] + new Vector3(0, playerPrefabs[type].transform.position.y, 0),
				playerPrefabs[type].transform.rotation);
			curPlayerSpawnPosIndex++;
			if (curPlayerSpawnPosIndex > 2)
			{
				playerSpawnable = false;
				indicator.SetActive(false);
			}
		}
	}

	void SetIndicator(int index)
	{
		if(curPlayerSpawnPosIndex <= 2)
		{
			indicator.transform.position = playerSpawnPos[index];
		}
	}

	void SpawnEnemy()
	{
		RestartPlayerSpawn();
		for (int i = 0; i < ENEMY_NUM; i++)
		{
			Instantiate(enemySpawnIndicator, enemySpawnPos[i], enemySpawnIndicator.transform.rotation);

			int randomIndex = Random.Range(0, enemiesPrefabs.Length);
			Instantiate(
				enemiesPrefabs[randomIndex],
				enemySpawnPos[i] + new Vector3(0, enemiesPrefabs[randomIndex].transform.position.y, 0),
				enemiesPrefabs[randomIndex].transform.rotation);
		}
		// Player player = GameObject.Find("Player").GetComponent<Player>();
        //player.UpdateGaugeBar(100f);
    }

	void RestartPlayerSpawn()
	{
        curPlayerSpawnPosIndex = 0;
        SetIndicator(curPlayerSpawnPosIndex);
        playerSpawnable = true;
        indicator.SetActive(true);
    }

	void ShiftPlayerPos()
	{
		GameObject[] curPlayerElements = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject curPlayerElement in curPlayerElements)
		{
			//curPlayerElement.transform.position += new Vector3(0, 0, SHIFT_PLAYER_POS_Y);
			StartCoroutine(MovePlayerUnitForward(curPlayerElement));
		}
	}

	IEnumerator MovePlayerUnitForward(GameObject gameObject)
	{
		if(gameObject != null)
		{
			float count = 0;
			float spf = 1.0f / 60.0f;
			float upf = 0.15f;
			while (count < SHIFT_PLAYER_POS_Y)
			{
				gameObject.transform.Translate(0, 0, upf);
				count += upf;
				yield return new WaitForSeconds(spf);
			}
		}
	}

	public void RestartGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	void SetEnemyWall()
	{
		Color wallColor;
		switch (difficulty)
		{
			case LEVEL_EASY:
				enemyWall.transform.position = new Vector3(0, 0, -SHIFT_PLAYER_POS_Y * 2);
				wallColor = colorEasy;
				break;
			case LEVEL_NORMAL:
				enemyWall.transform.position = new Vector3(0, 0, 0);
				wallColor = colorNormal;
				break;
			case LEVEL_HARD:
				enemyWall.transform.position = new Vector3(0, 0, SHIFT_PLAYER_POS_Y * 2);
				wallColor = colorHard;
				break;
			default:
				enemyWall.transform.position = new Vector3(0, 0, 0);
				wallColor = colorNormal;
				break;
		}
		var main = enemyWall.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
		main.startColor = wallColor;
		enemyWall.SetActive(true);
	}

	public float SetEnemySpeed()
	{
		float speed;
		switch(difficulty)
		{
            case LEVEL_EASY:
				speed = ENEMY_SPEED_EASY;
                break;
            case LEVEL_NORMAL:
                speed = ENEMY_SPEED_MEDIUM;
                break;
            case LEVEL_HARD:
                speed = ENEMY_SPEED_HARD;
                break;
            default:
				speed = ENEMY_SPEED_MEDIUM;
                break;
        }

		return speed;
	}

	public void StartAttackMode(int difficultIndex)
	{
		SetGameStartCounter(MODE_ATTACK, difficultIndex);
	}

    public void StartDefenseMode(int difficultIndex)
    {
        SetGameStartCounter(MODE_DEFENSE, difficultIndex);
    }

    private void SetGameStartCounter(int mode, int difficultIndex)
	{
        gameMenuManager.SetModeListScreen(false);
		gameMenuManager.DestroyTitleText();

        StartCoroutine(StartCounter(mode, difficultIndex));
	}

	IEnumerator StartCounter(int mode, int difficultIndex)
	{
        TextMeshProUGUI startCounterTextComponent = gameMenuManager.StartCounter.GetComponent<TextMeshProUGUI>();
		int count = SEC_COUNT_BEFORE_START;
		while (count >= 0)
		{
			string textToPrint = count.ToString();
			if (count == 0)
			{
				textToPrint = "GO!";
			}
			startCounterTextComponent.text = textToPrint;
			count--;
			yield return new WaitForSeconds(0.7f);
		}
		StartGame(mode, difficultIndex);
		Destroy(gameMenuManager.StartCounter);
    }

	void StartGame(int modeIndex, int difficultIndex)
	{
		isGameOver = false;
		playerSpawnable = true;
		difficulty = difficultIndex;
		mode = modeIndex;
        SetUIInGameByMode();
    }

	void SetUIInGameByMode()
	{
		switch(mode)
		{
			case GameManager.MODE_ATTACK:
				playerHPBar.SetActive(true);
				enemyHPBar.SetActive(true);
                ingameInstruction.SetActive(true);
				break;
			case GameManager.MODE_DEFENSE:
				SetEnemyWall();
                ingameInstruction.SetActive(true);
                break;
		}
	}
	public void SetWinScreen()
	{
		gameMenuManager.SetWinScreen(true);
		SetFirework();
	}

	public void SetGameOverMenu()
	{
		gameMenuManager.SetGameOverScreen(true);
	}

	private void SetFirework()
	{
        Instantiate(firework, fireworkPos, firework.transform.rotation);
    }
}
