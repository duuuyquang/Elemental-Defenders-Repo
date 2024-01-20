using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	const int ENEMY_NUM = 3;

	const float ENEMY_SPAWN_POS_Z = 12;

	const int SHIFT_PLAYER_POS_Y = 3;

	public const int LEVEL_EASY = 1;
	public const int LEVEL_NORMAL = 2;
	public const int LEVEL_HARD = 3;

	const float ENEMY_SPEED_EASY = 3f;
	const float ENEMY_SPEED_MEDIUM = 7f;
	const float ENEMY_SPEED_HARD = 10f;
	const float ENEMY_SPEED_MAX = 13f;

	const float ENEMY_REGEN_RATE_EASY = 0.25f;
	const float ENEMY_REGEN_RATE_MEDIUM = 0.75f;
	const float ENEMY_REGEN_RATE_HARD = 1.25f;

	const float PLAYER_REGEN_RATE_EASY = 1.5f;
	const float PLAYER_REGEN_RATE_MEDIUM = 1f;
	const float PLAYER_REGEN_RATE_HARD = 0.5f;

	const float ENEMY_SPAWN_DELAY_SEC = 0.4f;
	const int SEC_COUNT_BEFORE_START = 3;

	const int TIME_LIMIT_SEC = 120;
	const int TIMER_DEFAULT_VALUE = -2;

	public const int MODE_ATTACK = 1;
	public const int MODE_DEFENSE = 2;
	public const int MODE_ENDLESS = 3;

	public const int SCORE_GAIN_TYPE_ADVANTAGE = 3;
	public const int SCORE_GAIN_TYPE_SAME = 1;
	public const int SCORE_GAIN_TYPE_HIT_WALL = 10;

	public const float GAUGE_POINT_HIT_WALL = 30f;
	public const float GAUGE_POINT_ADVANTAGE = 10f;
	public const float GAUGE_POINT_SAME = 3f;

	public const int SEC_DELAY_AFTER_SHOOT = 2;

	public const int COMBO_BEGIN = 1;
    public const int COMBO_SHORT = 4;
    public const int COMBO_MEDIUM = 7;
    public const int COMBO_LONG = 10;

	const int COMBO_TEXT_BEGIN = 15;
	const int COMBO_TEXT_INCREASE_STEP = 1;

	public GameObject[] enemiesPrefabs;
	public GameObject indicator;
	public GameObject enemyWall;
	public GameObject playerHPBar;
	public GameObject enemyHPBar;
	public GameObject firework;
	public GameObject gaugeInfo;
	public GameObject spawnInstruction;
	public GameObject scoreGainPrefab;
	public GameObject highComboPartical;

	public TextMeshProUGUI timeText;

	private GameMenuManager gameMenuManager;
	private Player player;
	private BonusGauge bonusGauge;
	private SoundController soundController;
	private GameObject playerSetting;

	private Color colorEasy = new Color(0.74f, 0.7f, 0.05f);
	private Color colorNormal = new Color(1.0f, 0.64f, 0.0f);
	private Color colorHard = new Color(0.52f, 0.0f, 0.73f);

	private Vector3 fireworkPos = new Vector3(0, -6, -13);

	private Vector3[] enemySpawnPos =
	{
		new Vector3(-6, 0, ENEMY_SPAWN_POS_Z),
		new Vector3( 0, 0, ENEMY_SPAWN_POS_Z),
		new Vector3( 6, 0, ENEMY_SPAWN_POS_Z)
	};

	private TextMeshPro scoreText;
	private TextMeshPro comboText;
	private TextMeshPro comboScoreText;
	private TextMeshPro turnText;

    private bool playerSpawnable = false;
	private bool isGameOver = true;
	private int difficulty;
	private int mode;
	private int score = 0;
	private int timer;
	private bool showInstruction = true;
	private bool isPause = false;
	private int curTurn;

	public int CurTurn { 
		get { return curTurn; }  
		set {
			curTurn = Mathf.Max(0,value); 
		} 
	}

	public int Difficulty { get { return difficulty; } }

	public bool PlayerSpawnable
	{
		get { return playerSpawnable; }
		set { playerSpawnable = value; }
	}

	public bool GameOver
	{
		get
		{
			return isGameOver;
		}
		set
		{
			isGameOver = value;
		}
	}

	public int GameMode { get { return mode; } }

	public int Score
	{
		get { return score; }
		set
		{
			score = Math.Max(0, value);
		}
	}

	void Start()
	{
		gameMenuManager = GameObject.Find("GameMenuManager").GetComponent<GameMenuManager>();
		soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
		player = GameObject.Find("Player").GetComponent<Player>();
		playerSetting = GameObject.Find("PlayerSetting");
		comboText = GameObject.Find("ComboText").GetComponent<TextMeshPro>();
		scoreText = GameObject.Find("ScoreText").GetComponent<TextMeshPro>();
		comboScoreText = GameObject.Find("ComboScoreText").GetComponent<TextMeshPro>();
		turnText = GameObject.Find("TurnText").GetComponent<TextMeshPro>();
        timer = TIMER_DEFAULT_VALUE;
		curTurn = 0;
	}

	void Update()
	{
        if (!isGameOver)
		{
			DisplayScore();
            DisplayTurn();
            DisplayTime();
			UpdateGameByMode();
			HandlePlayerInput();
		}

		if (Input.GetKeyDown(KeyCode.G))
		{
			ToggleInstruction();
		}
	}

	private void ToggleInstruction()
	{
		if (showInstruction)
		{
			showInstruction = !showInstruction;
			spawnInstruction.SetActive(false);
		}
		else
		{
			showInstruction = !showInstruction;
			spawnInstruction.SetActive(true);
		}
	}

	private void DisplayTime()
	{
		if (timer >= 0)
		{
			timeText.text = "Time Left: " + timer;
		}
	}

	void PauseGame()
	{
		Time.timeScale = 0;
		Enemy.ToggleDisplayAllEnemyUnit(false);
	}
	void ResumeGame()
	{
		Time.timeScale = 1;
		Enemy.ToggleDisplayAllEnemyUnit(true);
	}

	void TimerColdown()
	{
		timer--;
		if (timer < 0)
		{
			isGameOver = true;
			SetGameOverMenu();
			Enemy.ClearAllEnemyUnit();
			CancelInvoke();
		}
	}

	public bool HasScoreSystem()
	{
		return mode != MODE_DEFENSE;
	}

	public void DisplayScore()
	{
        scoreText.text = "";
        if (HasScoreSystem())
		{
            scoreText.text = "Score: " + score;
        }
	}

	void DisplayTurn()
	{
        turnText.text = "";
		if(mode == MODE_ENDLESS) {
            turnText.text = "Turn: " + curTurn;
        }
	}

	public void DisplayCombo(int combo)
	{
		player.PerfectChain = combo;
		if (combo < COMBO_BEGIN)
		{
			comboText.fontSize = COMBO_TEXT_BEGIN;
			comboText.color = Color.white;
			comboText.text = "";
			DisplayComboScore(0);
			return;
		}
		else if (combo < COMBO_SHORT)
		{
			comboText.color = Color.cyan;
		}
		else if (combo < COMBO_MEDIUM)
		{
			comboText.color = Color.green;
		}
		else if (combo < COMBO_LONG)
		{
			comboText.color = Color.red;
		}
		else
		{
			comboText.color = Color.magenta;
			highComboPartical.SetActive(false);
			highComboPartical.SetActive(true);
		}

		if (combo <= COMBO_LONG)
		{
			comboText.fontSize += COMBO_TEXT_INCREASE_STEP;
		}
		comboText.text = "Combo x" + combo;
        BouncyComboTextEffect();
        DisplayComboScore(combo);
	}

	public void DisplayComboScore(int chain)
	{
		int score = ConvertChainToScore(chain);
		string displayText = "";
		if (score > 0)
		{
			displayText = "+" + score;
		}
		comboScoreText.text = displayText;
		comboScoreText.color = Color.yellow;
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
				if (enemyNum <= 0 && !player.IsAttacking)
				{
					RestartPlayerSpawn();
					SpawnEnemy();
				}
				break;
			case MODE_DEFENSE:
				if (timer == TIMER_DEFAULT_VALUE)
				{
					StartTimeColdown();
				}
				if (enemyNum <= 0)
				{
					RestartPlayerSpawn();
					SpawnEnemy();
					ShiftPlayerPos();
				}
				break;
            case MODE_ENDLESS:
                if (enemyNum <= 0 && !player.IsAttacking)
                {
                    RestartPlayerSpawn();
                    SpawnEnemy();
                    curTurn++;
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
			player.SpawnElement(Element.TYPE_FIRE);
			SetIndicator(player.CurPlayerSpawnPosIndex);
		}

		if (Input.GetKeyDown(KeyCode.W))
		{
			player.SpawnElement(Element.TYPE_WATER);
			SetIndicator(player.CurPlayerSpawnPosIndex);
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			player.SpawnElement(Element.TYPE_WOOD);
			SetIndicator(player.CurPlayerSpawnPosIndex);
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			isPause = !isPause;
			if (isPause)
			{
				gameMenuManager.SetPauseScreen(true);
				gameMenuManager.SetSoundOption(true);
				PauseGame();
			}
			else
			{
				gameMenuManager.SetPauseScreen(false);
				gameMenuManager.SetSoundOption(false);
				ResumeGame();
			}

		}
	}

	void SetIndicator(int index)
	{
		switch (mode)
		{
			case MODE_ATTACK:
			case MODE_ENDLESS:
				if (!bonusGauge)
				{
					bonusGauge = GameObject.Find("BonusGauge").GetComponent<BonusGauge>();
				}

				if (player.CurPlayerSpawnPosIndex <= 2)
				{
					indicator.transform.position = player.PlayerSpawnPos[index];
					bonusGauge.RegenGaugeByPercentage(ENEMY_SPAWN_DELAY_SEC * 10);
				}

				if (index == 0)
				{
					bonusGauge.ResetGauge();
				}
				break;

			case MODE_DEFENSE:
				if (player.CurPlayerSpawnPosIndex <= 2)
				{
					indicator.transform.position = player.PlayerSpawnPos[index];
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
				enemySpawnPos[i],
				enemiesPrefabs[randomIndex].transform.rotation);
			yield return new WaitForSeconds(seconds);
		}
	}

	void RestartPlayerSpawn()
	{
		player.CurPlayerSpawnPosIndex = 0;
		SetIndicator(player.CurPlayerSpawnPosIndex);
		playerSpawnable = true;
	}

	void ShiftPlayerPos()
	{
		GameObject[] curPlayerElements = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject curPlayerElement in curPlayerElements)
		{
			StartCoroutine(MovePlayerUnitForward(curPlayerElement));
		}
	}

	IEnumerator MovePlayerUnitForward(GameObject gameObject)
	{
		if (gameObject != null)
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
		ResumeGame(); // in case player pause game to restart
		DontDestroyOnLoad(playerSetting);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}

	void SetEnemyWall()
	{
        var main = enemyWall.transform.GetComponentInChildren<ParticleSystem>().main;
        switch (mode)
		{
			case MODE_DEFENSE:
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
                main.startColor = wallColor;
                break;
			case MODE_ATTACK:
            case MODE_ENDLESS:
                enemyWall.transform.position = new Vector3(0, 0, 11.5f);
                main.startColor = colorHard;
                break;
		}
	}

	public float GetEnemySpeed()
	{
		float speed;

		switch (difficulty)
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

        if (GameMode == MODE_ENDLESS)
        {
            speed = Mathf.Min(speed + curTurn/2, ENEMY_SPEED_MAX);
        }

        return speed;
	}

	public void StartEndlessMode()
	{
        SetGameStartCounter(MODE_ENDLESS, LEVEL_EASY); // always start at easy
        soundController.PlayButtonClick();
    }

	public void StartAttackMode(int difficultIndex)
	{
		SetGameStartCounter(MODE_ATTACK, difficultIndex);
		soundController.PlayButtonClick();
	}

	public void StartDefenseMode(int difficultIndex)
	{
		SetGameStartCounter(MODE_DEFENSE, difficultIndex);
		soundController.PlayButtonClick();
	}

	private void SetGameStartCounter(int mode, int difficultIndex)
	{
		gameMenuManager.SetStartScreen(false);
		gameMenuManager.SetSoundOption(false);
		StartCoroutine(StartCounter(mode, difficultIndex));
	}

	IEnumerator StartCounter(int mode, int difficultIndex)
	{
		yield return new WaitForSeconds(0.7f);
		TextMeshProUGUI startCounterTextComponent = gameMenuManager.StartCounter.GetComponent<TextMeshProUGUI>();
		int count = SEC_COUNT_BEFORE_START;
		while (count >= 0)
		{
			string textToPrint = count.ToString();
			if (count == 0)
			{
				textToPrint = "GO!";
				soundController.PlayStartSound();
			}
			startCounterTextComponent.text = textToPrint;
			count--;
			soundController.PlayCountSound();
			yield return new WaitForSeconds(0.7f);
		}
		Destroy(gameMenuManager.StartCounter);
		StartGame(mode, difficultIndex);
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
		switch (mode)
		{
			case MODE_ATTACK:
				SetEnemyWall();
				gameMenuManager.SetInGameScreen(true);
				playerHPBar.SetActive(true);
				enemyHPBar.SetActive(true);
				indicator.SetActive(true);
				spawnInstruction.SetActive(true);
				gaugeInfo.SetActive(true);
				break;
			case MODE_DEFENSE:
				SetEnemyWall();
				gameMenuManager.SetInGameScreen(true);
				indicator.SetActive(true);
				spawnInstruction.SetActive(true);
				break;
			case MODE_ENDLESS:
                SetEnemyWall();
                gameMenuManager.SetInGameScreen(true);
                playerHPBar.SetActive(true);
                indicator.SetActive(true);
                spawnInstruction.SetActive(true);
                gaugeInfo.SetActive(true);
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

	public void BlockPlayerSpawn()
	{
		playerSpawnable = false;
		indicator.SetActive(false);
	}

	public int ConvertChainToScore(int perfectChain)
	{
		int firstIndex = perfectChain / 10;
		int secondIndex = perfectChain % 10;
		if (firstIndex > 0)
		{
			firstIndex *= 10;
		}

		int baseScore = SCORE_GAIN_TYPE_ADVANTAGE * perfectChain;

		int score = (int)Mathf.Ceil(baseScore * (1.0f + 0.1f * (firstIndex + secondIndex)));

		return score;
	}

	public float GetPlayerCurrentRegenRate()
	{
		float rate;
		switch (difficulty)
		{
			case LEVEL_EASY:
				rate = PLAYER_REGEN_RATE_EASY;
				break;
			case LEVEL_NORMAL:
				rate = PLAYER_REGEN_RATE_MEDIUM;
				break;
			case LEVEL_HARD:
				rate = PLAYER_REGEN_RATE_HARD;
				break;
			default:
				rate = PLAYER_REGEN_RATE_EASY;
				break;
		}
		return rate;
	}

	public float GetEnemyCurrentRegenRate()
	{
		float rate;
		switch (difficulty)
		{
			case LEVEL_EASY:
				rate = ENEMY_REGEN_RATE_EASY;
				break;
			case LEVEL_NORMAL:
				rate = ENEMY_REGEN_RATE_MEDIUM;
				break;
			case LEVEL_HARD:
				rate = ENEMY_REGEN_RATE_HARD;
				break;
			default:
				rate = ENEMY_REGEN_RATE_EASY;
				break;
		}
		return rate;
	}

	public void DisplayScoreGain(Vector3 position, int point)
	{
		TextMeshPro text = scoreGainPrefab.GetComponent<TextMeshPro>();
		text.text = "+" + point;
		Instantiate(scoreGainPrefab, position, scoreGainPrefab.transform.rotation);
		//BouncyScoreTextEffect();
        JumpScoreEffect(point);
    }

	public void BouncyScoreTextEffect()
	{
		StartCoroutine(TextBouncyEffect(scoreText));
	}

	public void BouncyComboTextEffect()
	{
		StartCoroutine(TextBouncyEffect(comboText));
	}

	public void JumpScoreEffect(int point)
	{
		StartCoroutine(ScoreIncreasingAnimation(point));
	}

	IEnumerator ScoreIncreasingAnimation(int point)
	{
        if (score < point)
        {
            yield return false;
        }

        int addNum = point;
		int minusStep = 1;
		if(point > 100)
		{
			addNum = 100;
        }

		if(point <= 10 && score > 9 )
		{
			addNum = 9;
		}

        score -= addNum;

        while (addNum > 0)
		{
            score += minusStep;
            addNum -= minusStep;
			yield return new WaitForNextFrameUnit();
        }
    }

    IEnumerator TextBouncyEffect(TextMeshPro text)
	{
		int upSize = 4;
		text.fontSize += upSize;
		while(upSize > 0)
		{
            yield return new WaitForNextFrameUnit();
            text.fontSize--;
			upSize--;
        }
    }
}
