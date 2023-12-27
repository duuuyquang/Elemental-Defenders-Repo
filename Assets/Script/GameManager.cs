using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    const int   ENEMY_NUM = 3;
    const float ENEMY_SPAWN_POS_Y = 12;
    const float PLAYER_SPAWN_POS_Y = 17;
    const int   SHIFT_PLAYER_POS_Y = 3;

    const int LEVEL_EASY = 1;
    const int LEVEL_NORMAL = 2;
    const int LEVEL_HARD = 3;

    public GameObject[] enemiesPrefabs;
    public GameObject[] playerPrefabs;
    public GameObject indicator;
    public GameObject rsBtn;
    public GameObject gameOverText;
    public GameObject winText;
    public GameObject enemySpawnIndicator;
    public GameObject enemyWall;

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

    // Start is called before the first frame update
    void Start()
    {
        SetIndicator(curPlayerSpawnPosIndex);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGameOver)
        {
            int enemyNum = GameObject.FindGameObjectsWithTag("Enemy").Length;
            if (enemyNum <= 0)
            {
                SpawnEnemy();
                ShiftPlayerPos();
            }

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
        curPlayerSpawnPosIndex = 0;
        SetIndicator(curPlayerSpawnPosIndex);
        playerSpawnable = true;
        indicator.SetActive(true);
        for (int i = 0; i < ENEMY_NUM; i++)
        {
            Instantiate(enemySpawnIndicator, enemySpawnPos[i], enemySpawnIndicator.transform.rotation);

            int randomIndex = Random.Range(0, enemiesPrefabs.Length);
            Instantiate(
                enemiesPrefabs[randomIndex],
                enemySpawnPos[i] + new Vector3(0, enemiesPrefabs[randomIndex].transform.position.y, 0),
                enemiesPrefabs[randomIndex].transform.rotation);
        }
    }

    void ShiftPlayerPos()
    {
        GameObject[] curPlayerElements = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject curPlayerElement in curPlayerElements)
        {
            //curPlayerElement.transform.position += new Vector3(0, 0, SHIFT_PLAYER_POS_Y);
            //curPlayerElement.transform.Translate(0, 0, 3);
            StartCoroutine(MovePlayer(curPlayerElement));
        }
    }

    IEnumerator MovePlayer(GameObject gameObject)
    {
        if(gameObject != null)
        {
            float count = 0;
            float spf = 1.0f / 60.0f;
            while (count < SHIFT_PLAYER_POS_Y)
            {
                gameObject.transform.Translate(0, 0, 0.15f);
                count += 0.15f;
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
                wallColor = new Color(0.74f, 0.7f, 0.05f);
                break;
            case LEVEL_NORMAL:
                enemyWall.transform.position = new Vector3(0, 0, 0);
                wallColor = new Color(1.0f, 0.64f, 0.0f);
                break;
            case LEVEL_HARD:
                enemyWall.transform.position = new Vector3(0, 0, SHIFT_PLAYER_POS_Y * 2);
                wallColor = new Color(0.52f, 0.0f, 0.73f);
                break;
            default:
                enemyWall.transform.position = new Vector3(0, 0, 0);
                wallColor = new Color(248, 124, 29);
                break;
        }
        var main = enemyWall.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
        main.startColor = wallColor;
        enemyWall.SetActive(true);
    }

    public void StartGame(int mode)
    {
        isGameOver = false;
        playerSpawnable = true;
        difficulty = mode;
        SetEnemyWall();

        GameObject startBtn = GameObject.Find("LevelButtons");
        GameObject titleText = GameObject.Find("TitleText");
        startBtn.SetActive(false);
        titleText.SetActive(false);
    }

    public void GameOver()
    {
        isGameOver = true;
    }

    public void SetWinScreen()
    {
        winText.SetActive(true);
        rsBtn.SetActive(true);
    }

    public void SetGameOverMenu()
    {
        rsBtn.SetActive(true);
        gameOverText.SetActive(true);
    }
}
