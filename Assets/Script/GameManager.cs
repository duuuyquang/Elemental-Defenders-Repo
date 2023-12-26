using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    const int ENEMY_NUM = 3;
    public GameObject[] enemiesPrefabs;
    public GameObject[] playerPrefabs;
    public GameObject indicator;
    public GameObject rsBtn;
    public GameObject gameOverText;
    public GameObject enemySpawnIndicator;

    private Vector3[] enemySpawnPos =
    {
        new Vector3(-6, 0, 12),
        new Vector3(0, 0, 12),
        new Vector3(6, 0, 12)
    };

    private Vector3[] playerSpawnPos =
    {
        new Vector3(-6, 0, -11),
        new Vector3(0, 0, -11),
        new Vector3(6, 0, -11)
    };

    private int curPlayerSpawnPosIndex = 0;
    private bool playerSpawnable = true;
    private bool isGameOver = true;

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
            curPlayerElement.transform.position += new Vector3(0, 0, 2);
        }
    }

    public void onRestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void StartGame()
    {
        isGameOver = false;
        GameObject startBtn = GameObject.Find("StartButton");
        GameObject titleText = GameObject.Find("TitleText");
        startBtn.SetActive(false);
        titleText.SetActive(false);
    }

    public void GameOver()
    {
        isGameOver = true;
    }

    public void SetGameOverMenu() {
        rsBtn.SetActive(true);
        gameOverText.SetActive(true);
    }
}
