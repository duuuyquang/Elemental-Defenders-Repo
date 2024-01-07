using System.Collections;
using TMPro;
using UnityEngine;

public class OnTouchEnemy : MonoBehaviour
{
    private Element element;

    public GameObject playerExplosionPrefab;
    public GameObject scoreGainPrefab;

    public int elementType;

    private GameManager gameManager;
    private Player player;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player").GetComponent<Player>();

        switch (elementType)
        {
            case Element.TYPE_FIRE:
                element = new Fire();
                break;
            case Element.TYPE_WATER:
                element = new Water();
                break;
            case Element.TYPE_WOOD:
                element = new Wood();
                break;
        }
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * 10);
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
                if (element.GetTypeAdvantage(enemy.ElementType) != Element.TYPE_STRONGER)
                {
                    PlayerExplosion();
                }
                break;

            case GameManager.MODE_ATTACK:
                if (element.GetTypeAdvantage(enemy.ElementType) == Element.TYPE_STRONGER)
                {
                    PlayerExplosion();
                    gameManager.Score += GameManager.SCORE_GAIN_TYPE_ADVANTAGE;
                    DisplayScoreGain(GameManager.SCORE_GAIN_TYPE_ADVANTAGE);
                    SetPlayerGauge(GameManager.GAUGE_POINT_ADVANTAGE);
                    player.PerfectChain++;
                }
                else if (element.GetTypeAdvantage(enemy.ElementType) == Element.TYPE_WEAKER)
                {
                    player.PerfectChain = 0;
                    PlayerExplosion();
                }
                else
                {
                    PlayerExplosion();
                    gameManager.Score += GameManager.SCORE_GAIN_TYPE_SAME;
                    player.PerfectChain = 0;
                    DisplayScoreGain(GameManager.SCORE_GAIN_TYPE_SAME);
                    SetPlayerGauge(GameManager.GAUGE_POINT_SAME);
                }
                break;
        }
    }

    void SetPlayerGauge(float value)
    {
        player.CurGauge += value;
        player.UpdateGaugeBar(player.CurGauge);
    }

    void DisplayScoreGain(int point)
    {
        TextMeshPro text = scoreGainPrefab.GetComponent<TextMeshPro>();
        text.text = "+" + point;
        Instantiate(scoreGainPrefab, transform.position, scoreGainPrefab.transform.rotation);
    }

    void PlayerExplosion()
    {
        Destroy(gameObject);
        Instantiate(playerExplosionPrefab, transform.position, playerExplosionPrefab.transform.rotation);
    }
}
