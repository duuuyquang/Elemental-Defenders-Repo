using System.Collections;
using TMPro;
using UnityEngine;

public class OnTouchEnemy : MonoBehaviour
{
    private Element playerElement;

    public GameObject playerExplosionPrefab;
    public GameObject enemyExplosionPrefab;
    public GameObject scorePrefab;

    public int playerElementType;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        switch (playerElementType)
        {
            case Element.TYPE_FIRE:
                playerElement = new Fire();
                break;
            case Element.TYPE_WATER:
                playerElement = new Water();
                break;
            case Element.TYPE_WOOD:
                playerElement = new Wood();
                break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            ProcessCollisionOnEnemy(collision);
        }
    }

    private void ProcessCollisionOnEnemy(Collision collision)
    {
        GameObject enemyObj = collision.gameObject;
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        switch (gameManager.GameMode)
        {
            case GameManager.MODE_DEFENSE:

                if (playerElement.GetTypeAdvantage(enemy.ElementType) == Element.TYPE_STRONGER)
                {
                    EnemyExplosive(enemyObj);
                }
                else if (playerElement.GetTypeAdvantage(enemy.ElementType) == Element.TYPE_WEAKER)
                {
                    PlayerExplosive();
                }
                else
                {
                    Destroy(enemyObj);
                    PlayerExplosive();
                }
                break;

            case GameManager.MODE_ATTACK:
                if (playerElement.GetTypeAdvantage(enemy.ElementType) == Element.TYPE_STRONGER)
                {
                    EnemyExplosive(enemyObj);
                    PlayerExplosive();
                    gameManager.Score += GameManager.SCORE_GAIN_TYPE_ADVANTAGE;
                    DisplayScoreGainEffect(GameManager.SCORE_GAIN_TYPE_ADVANTAGE);
                }
                else if (playerElement.GetTypeAdvantage(enemy.ElementType) == Element.TYPE_WEAKER)
                {
                    PlayerExplosive();
                }
                else
                {
                    Destroy(enemyObj);
                    PlayerExplosive();
                    gameManager.Score += GameManager.SCORE_GAIN_TYPE_SAME;
                    DisplayScoreGainEffect(GameManager.SCORE_GAIN_TYPE_SAME);
                }
                break;
        }
    }

    void DisplayScoreGainEffect(int point)
    {
        TextMeshPro text = scorePrefab.GetComponent<TextMeshPro>();
        text.text = "+" + point;
        Instantiate(scorePrefab, transform.position, scorePrefab.transform.rotation);
    }

    void PlayerExplosive()
    {
        Destroy(gameObject);
        Instantiate(playerExplosionPrefab, transform.position, playerExplosionPrefab.transform.rotation);
    }

    void EnemyExplosive(GameObject enemyObject)
    {
        Destroy(enemyObject);
        Instantiate(enemyExplosionPrefab, transform.position, enemyExplosionPrefab.transform.rotation);
    }
}
