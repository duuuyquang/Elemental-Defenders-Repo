using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BonusGauge : MonoBehaviour
{
    const float DELTA_VALUE_SCALE_TO_POS = 1.8f; // update this variable to adjust the duration of bonus time - higher = shorter duration
    public float initialScaleX = 3f;

    public TextMeshPro bonusScoreText;
    public GameObject scoreGainPrefab;

    private static Color COLOR_COMBO_SHORT = new Color(0f, 1f, 1f);
    private static Color COLOR_COMBO_MEDIUM = new Color(1f, 1f, 0f);
    private static Color COLOR_COMBO_LONG = new Color(1f, 0.5f, 0f);

    private const int BONUS_MAX    = 4;
    private const int BONUS_MEDIUM = 2;
    private const int BONUS_MIN    = 1;

    private const float RATE_BONUS_MAX     = 0.7f;
    private const float RATE_BONUS_MEDIUM  = 0.4f;
    private const float RATE_BONUS_MIN     = 0f;


    private int curBonus = 3;
    private float speed;
    private bool startColdown = false;

    private GameManager gameManager;
    public Material material;

    public int CurBonus {  get { return curBonus; } set { curBonus = value; } }

    public bool StartColdown { get { return startColdown;  } set { startColdown = value; } }

    private void Start()
    {
        transform.localScale = new Vector3(initialScaleX, 0.2f, 1);
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if(gameManager.GameMode == GameManager.MODE_DEFENSE)
        {
            gameObject.SetActive(false);
            return;
        }

        float enemySpeed = gameManager.GetEnemySpeed();
        GameObject enemyWall = GameObject.Find("EnemyWallInfo");
        GameObject playerWall = GameObject.Find("PlayerWallInfo");
        float distance = (enemyWall.transform.position.z - playerWall.transform.position.z);

        speed = initialScaleX * enemySpeed / distance;
    }

    // Update is called once per frame
    void Update()
    {
        if(startColdown)
        {
            Coldown();
        }
    }

    void Coldown()
    {
        if (transform.localScale.x > 0)
        {
            transform.localScale -= new Vector3(speed * Time.deltaTime * DELTA_VALUE_SCALE_TO_POS, 0, 0);
        }
        else
        {
            startColdown = false;
        }
        UpdateBonusTextAndGaugeColor();
    }

    void UpdateCurrentBonusScore()
    {
        float currentRate = transform.localScale.x / initialScaleX;
        curBonus = 0;
        if (currentRate > RATE_BONUS_MAX)
        {
            curBonus = BONUS_MAX;
        }
        else if (currentRate > RATE_BONUS_MEDIUM)
        {
            curBonus = BONUS_MEDIUM;
        }
        else if (currentRate > RATE_BONUS_MIN)
        {
            curBonus = BONUS_MIN;
        }
    }

    void UpdateBonusTextAndGaugeColor()
    {
        UpdateCurrentBonusScore();
        bonusScoreText.text = "Bonus: " + (curBonus > 0 ? "+" + curBonus : 0);
        bonusScoreText.color = GetColorByScore(curBonus);
        material.color = GetColorByScore(curBonus);
    }

    public static Color GetColorByScore(int score)
    {
        Color curColor;
        switch (score)
        {
            case 0:
                curColor = Color.clear;
                break;
            case BONUS_MIN:
                curColor = COLOR_COMBO_SHORT;
                break;
            case BONUS_MEDIUM:
                curColor = COLOR_COMBO_MEDIUM;
                break;
            case BONUS_MAX:
                curColor = COLOR_COMBO_LONG;
                break;
            default:
                curColor = Color.clear;
                break;
        }

        return curColor;
    }

    public void DisplayBonusScoreGain(int score)
    {
        TextMeshPro text = scoreGainPrefab.GetComponent<TextMeshPro>();
        text.text = "+" + score;
        text.color = GetColorByScore(score);
        text.fontSize = 6;
        Instantiate(scoreGainPrefab, transform.position, scoreGainPrefab.transform.rotation);
    }

    public void ResetGauge()
    {
        startColdown = true;
        transform.localScale = new Vector3(initialScaleX, 0.2f, 1);
    }

    public void RegenGaugeByPercentage(float percent)
    {
        float value = initialScaleX * percent / 100;
        transform.localScale += new Vector3(value, 0, 0);
    }
}
