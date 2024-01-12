using TMPro;
using UnityEngine;

public class OnTouchEnemy : MonoBehaviour
{
    private Element element;

    [SerializeField] private float speed = 10f;

    public GameObject playerExplosionPrefab;
    public GameObject scoreGainPrefab;
    public GameObject bonusGainPrefab;

    public int bonusScore;
    public int elementType;

    private GameManager gameManager;
    private SoundController soundController;
    private Player player;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
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
        if(gameManager.GameMode == GameManager.MODE_ATTACK)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
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
        int enemyEleType = enemy.ElementType;
        switch (gameManager.GameMode)
        {
            case GameManager.MODE_DEFENSE:
                if (element.GetTypeAdvantage(enemyEleType) != Element.TYPE_STRONGER)
                {
                    TriggerExplosion();
                    soundController.PlayElementCollisionByType(SoundController.TYPE_STRONGER);
                }
                else if (element.GetTypeAdvantage(enemyEleType) == Element.TYPE_WEAKER)
                {
                    soundController.PlayElementCollisionByType(SoundController.TYPE_WEAKER);
                }
                else
                {
                    soundController.PlayElementCollisionByType(SoundController.TYPE_SAME);
                }
                break;

            case GameManager.MODE_ATTACK:
                if (element.GetTypeAdvantage(enemyEleType) == Element.TYPE_STRONGER)
                {
                    gameManager.Score += GameManager.SCORE_GAIN_TYPE_ADVANTAGE + bonusScore;
                    DisplayAllScoresGained(GameManager.SCORE_GAIN_TYPE_ADVANTAGE);
                    SetPlayerGauge(GameManager.GAUGE_POINT_ADVANTAGE);
                    gameManager.DisplayCombo(++player.PerfectChain);
                    soundController.PlayElementCollisionByType(SoundController.TYPE_STRONGER);
                }
                else if (element.GetTypeAdvantage(enemyEleType) == Element.TYPE_WEAKER)
                {
                    gameManager.DisplayCombo(0);
                    soundController.PlayElementCollisionByType(SoundController.TYPE_WEAKER);
                }
                else
                {
                    gameManager.Score += GameManager.SCORE_GAIN_TYPE_SAME + bonusScore;
                    DisplayAllScoresGained(GameManager.SCORE_GAIN_TYPE_SAME);
                    SetPlayerGauge(GameManager.GAUGE_POINT_SAME);
                    gameManager.DisplayCombo(0);
                    soundController.PlayElementCollisionByType(SoundController.TYPE_SAME);
                }
                TriggerExplosion();
                break;
        }
    }

    void SetPlayerGauge(float value)
    {
        player.CurGauge += value;
        player.UpdateGaugeBar(player.CurGauge);
    }

    public void DisplayAllScoresGained(int score)
    {
        if (bonusScore > 0)
        {
            DisplayBonusGain(bonusScore);
        }
        DisplayScoreGain(score);
    }

    void DisplayScoreGain(int point)
    {
        TextMeshPro text = scoreGainPrefab.GetComponent<TextMeshPro>();
        text.text = "+" + point;
        Instantiate(scoreGainPrefab, transform.position, scoreGainPrefab.transform.rotation);
    }
    void DisplayBonusGain(int point)
    {
        TextMeshPro text = bonusGainPrefab.GetComponent<TextMeshPro>();
        text.text = "+" + point;
        text.color = BonusGauge.GetColorByScore(point);
        Instantiate(bonusGainPrefab, transform.position + new Vector3(0,1,0), bonusGainPrefab.transform.rotation);
    }

    public void TriggerExplosion()
    {
        Destroy(gameObject);
        Instantiate(playerExplosionPrefab, transform.position, playerExplosionPrefab.transform.rotation);
    }
}
