using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	const int ENEMY_NUM = 3;

	const float ENEMY_SPAWN_POS_Z	= 12;
	const float PLAYER_SPAWN_POS_Z	= 17;

	const int SHIFT_PLAYER_POS_Y	= 3;

	const int LEVEL_EASY	= 1;
	const int LEVEL_NORMAL	= 2;
	const int LEVEL_HARD	= 3;

	const float ENEMY_SPEED_EASY	= 3f;
	const float ENEMY_SPEED_MEDIUM	= 6f;
    const float ENEMY_SPEED_HARD	= 9f;

	const float ENEMY_SPAWN_DELAY_SEC = 0.4f;
    const int SEC_COUNT_BEFORE_START = 3;

	const int TIME_LIMIT_SEC = 180;
	const int TIMER_DEFAULT_VALUE = -2;

	public const int MODE_ATTACK	= 1;
	public const int MODE_DEFENSE	= 2;

	public const int SCORE_GAIN_TYPE_ADVANTAGE = 3;
	public const int SCORE_GAIN_TYPE_SAME = 1;
	public const int SCORE_GAIN_TYPE_HIT_WALL = 10;

	public const float GAUGE_POINT_HIT_WALL = 30f;
	public const float GAUGE_POINT_ADVANTAGE = 10f;
	public const float GAUGE_POINT_SAME = 3f;

	public GameObject[] enemiesPrefabs;
	public GameObject[] playerPrefabs;
	public GameObject indicator;
	public GameObject enemyWall;
	public GameObject ingameInstruction;
	public GameObject playerHPBar;
	public GameObject enemyHPBar;
    public GameObject firework;
	public GameObject gaugeInfo;

    public TextMeshProUGUI scoreText;
	public TextMeshProUGUI chainText;
	public TextMeshProUGUI chainScoreText;

    private GameMenuManager gameMenuManager;
	private Player player;
	private BonusGauge bonusGauge;

    private Color colorEasy		= new Color(0.74f, 0.7f, 0.05f);
    private Color colorNormal	= new Color(1.0f, 0.64f, 0.0f);
    private Color colorHard		= new Color(0.52f, 0.0f, 0.73f);

    private Vector3 fireworkPos = new Vector3(0, -6, -13);

    private Vector3[] enemySpawnPos =
	{
		new Vector3(-6, 0, ENEMY_SPAWN_POS_Z),
		new Vector3( 0, 0, ENEMY_SPAWN_POS_Z),
		new Vector3( 6, 0, ENEMY_SPAWN_POS_Z)
	};

	private Vector3[] playerSpawnPos =
	{
		new Vector3(-6, 0, -PLAYER_SPAWN_POS_Z),
		new Vector3(0, 0, -PLAYER_SPAWN_POS_Z),
		new Vector3(6, 0, -PLAYER_SPAWN_POS_Z)
	};

    private int curPlayerSpawnPosIndex = 0;
	private bool playerSpawnable = false;
	private bool isGameOver = true;
	private int difficulty;
	private int mode;
    private int score = 0;
	private int timer;

	public TextMeshProUGUI timeText;

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
        gameMenuManager = GameObject.Find("GameMenuManager").GetComponent<GameMenuManager>();
		player = GameObject.Find("Player").GetComponent<Player>();
		timer = TIMER_DEFAULT_VALUE;
    }

    void Update()
    {
        if (GameMode == MODE_ATTACK)
        {
            DisplayScore();
            DisplayTimeText();
        }

        if (!isGameOver)
        {
            UpdateGameByMode();
            HandlePlayerInput();
        }
    }

    private void DisplayTimeText()
    {
        if(timer >= 0)
		{
			timeText.text = "Time: " + timer;
		}
    }

    void TimerColdown()
	{
		timer--;
		if(timer < 0)
		{
            isGameOver = true;
			SetGameOverMenu();
			Enemy.ClearAllEnemyUnit();
			CancelInvoke();
        }
    }

	void TimeCounter()
	{
		timer++;
        if (isGameOver)
        {
            CancelInvoke();
        }
    }

	public void DisplayScore()
	{
		scoreText.text = "Score: " + score;
    }

	public void DisplayCombo()
	{
		if(player.PerfectChain <= 10)
		{
			if(player.PerfectChain < 1)
			{
				chainText.fontSize = 20;
				chainText.color = Color.white;
                chainText.text = "";
                DisplayComboScore();
				return;
            }
			else if(player.PerfectChain < 4)
			{
				chainText.color = Color.cyan;
			} 
			else if(player.PerfectChain < 7)
			{
				chainText.color = Color.yellow;
			} 
			else if(player.PerfectChain < 10)
			{
                chainText.color = Color.green;
            } 
			else
			{
				chainText.color = Color.red;
			}
            chainText.fontSize += 2;
        }
		chainText.text = "Combo x" + player.PerfectChain;
		DisplayComboScore();
    }

	public void DisplayComboScore()
	{
		string displayText = "";
        int score = ConvertChainToScore(player.PerfectChain);
        displayText = "+" + score;

		chainScoreText.text = displayText;
        chainScoreText.color = Color.green;

    }

    public void ResetChain()
    {
        player.PerfectChain = 0;
    }

    public void UpdatePerfectChain()
    {
        player.PerfectChain++;
    }
	
	private void StartTimeColdown()
	{
        timer = TIME_LIMIT_SEC;
        InvokeRepeating("TimerColdown", 1, 1);
    }

	private void StartTimeCounter()
	{
        timer = 0;
        InvokeRepeating("TimeCounter", 1, 1);
    }

    void UpdateGameByMode()
	{
		int enemyNum = GameObject.FindGameObjectsWithTag("Enemy").Length;
        switch (mode)
		{
			case MODE_ATTACK:
				if (timer == TIMER_DEFAULT_VALUE)
				{
                    StartTimeColdown();
                }
                if (enemyNum <= 0)
				{
					RestartPlayerSpawn();
                    SpawnEnemy();
                }
				break;
			case MODE_DEFENSE:
				if (enemyNum <= 0)
				{
                    RestartPlayerSpawn();
                    SpawnEnemy();
					ShiftPlayerPos();
				}
                break;
			default:
				return;
		}
	}

    void HandlePlayerInput()
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
			int bonusScore = 0;
			if (GameMode == MODE_ATTACK)
			{
				bonusScore = bonusGauge.CurBonus;
            }
            playerPrefabs[type].GetComponent<OnTouchEnemy>().bonusScore = bonusScore;

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
		switch(mode)
		{
			case MODE_ATTACK:
                if (!bonusGauge)
                {
                    bonusGauge = GameObject.Find("BonusGauge").GetComponent<BonusGauge>();
                }

                if (curPlayerSpawnPosIndex <= 2)
                {
                    indicator.transform.position = playerSpawnPos[index];
                    bonusGauge.RegenGaugeByPercentage(ENEMY_SPAWN_DELAY_SEC * 10);
                }

                if (index == 0)
                {
                    bonusGauge.ResetGauge();
                }
				break;
			case MODE_DEFENSE:
				if (curPlayerSpawnPosIndex <= 2)
                {
                    indicator.transform.position = playerSpawnPos[index];
                }
                break;
		}
	}

	void SpawnEnemy()
	{
        indicator.SetActive(true);
        StartCoroutine(SpawnEnemyByDelay(ENEMY_SPAWN_DELAY_SEC));
    }

	IEnumerator SpawnEnemyByDelay(float seconds)
	{
        for (int i = 0; i < ENEMY_NUM; i++)
        {
            int randomIndex = Random.Range(0, enemiesPrefabs.Length);
			Instantiate(
				enemiesPrefabs[randomIndex],
				enemySpawnPos[i] + new Vector3(0, enemiesPrefabs[randomIndex].transform.position.y, 0),
				enemiesPrefabs[randomIndex].transform.rotation);
			yield return new WaitForSeconds(seconds);
		}
	}

	void RestartPlayerSpawn()
	{
        curPlayerSpawnPosIndex = 0;
        SetIndicator(curPlayerSpawnPosIndex);
        playerSpawnable = true;
    }

	void ShiftPlayerPos()
	{
		GameObject[] curPlayerElements = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject curPlayerElement in curPlayerElements)
		{
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
		if(mode == MODE_DEFENSE)
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
            var main = enemyWall.transform.GetComponentInChildren<ParticleSystem>().main;
            main.startColor = wallColor;
        }
		else if( mode == MODE_ATTACK)
		{
            enemyWall.transform.position = new Vector3(0, 0, 6);
            var main = enemyWall.transform.GetComponentInChildren<ParticleSystem>().main;
            main.startColor = colorHard;
        }
        enemyWall.SetActive(true);
    }

    public float GetEnemySpeed()
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
			case MODE_ATTACK:
                SetEnemyWall();
                playerHPBar.SetActive(true);
				enemyHPBar.SetActive(true);
                indicator.SetActive(true);
				ingameInstruction.SetActive(true);
				gaugeInfo.SetActive(true);
				break;
			case MODE_DEFENSE:
				SetEnemyWall();
                indicator.SetActive(true);
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

    public int ConvertChainToScore(int perfectChain)
    {
        int firstIndex = perfectChain / 10;
        int secondIndex = perfectChain % 10;
		if(secondIndex == 0)
		{
			firstIndex *= 10;
        }

        int baseScore = SCORE_GAIN_TYPE_ADVANTAGE * perfectChain;

		int score = (int)Mathf.Ceil(baseScore * (1.0f + 0.1f * (firstIndex + secondIndex)));

		return score;
    }
}
