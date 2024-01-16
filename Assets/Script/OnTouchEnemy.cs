using System.Collections;
using TMPro;
using UnityEngine;

public class OnTouchEnemy : MonoBehaviour
{
    private Element element;

    [SerializeField] private float speed = 10f;

    public GameObject explosionPrefab;
    public GameObject gloryEffectPrefab;
    public GameObject scoreGainPrefab;
    public GameObject bonusGainPrefab;

    public int bonusScore;
    public int elementType;

    private GameManager gameManager;
    private SoundController soundController;
    private Player player;

    private Animator animator;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
        player = GameObject.Find("Player").GetComponent<Player>();
        animator = gameObject.GetComponentInChildren<Animator>();

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

        if (gameManager.GameMode == GameManager.MODE_DEFENSE)
        {
            animator.SetBool("isDefending", true);
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
                    animator.SetBool("isVictory", true);
                    StartCoroutine(AnimateAfterCollision(true));
                }
                else if (element.GetTypeAdvantage(enemyEleType) == Element.TYPE_WEAKER)
                {
                    gameManager.DisplayCombo(0);
                    soundController.PlayElementCollisionByType(SoundController.TYPE_WEAKER);
                    TriggerExplosion();
                }
                else
                {
                    gameManager.Score += GameManager.SCORE_GAIN_TYPE_SAME + bonusScore;
                    DisplayAllScoresGained(GameManager.SCORE_GAIN_TYPE_SAME);
                    SetPlayerGauge(GameManager.GAUGE_POINT_SAME);
                    gameManager.DisplayCombo(0);
                    soundController.PlayElementCollisionByType(SoundController.TYPE_SAME);
                    animator.SetBool("isDead", true);
                    StartCoroutine(AnimateAfterCollision(false));
                }
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
        Instantiate(explosionPrefab, transform.position, explosionPrefab.transform.rotation);
    }

    IEnumerator AnimateAfterCollision(bool isGlory)
    {
        Instantiate(explosionPrefab, transform.position, explosionPrefab.transform.rotation);
        speed = 0;
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + 1f);
        if (isGlory)
        {
            Instantiate(gloryEffectPrefab, transform.position + new Vector3(0, 1, 0), gloryEffectPrefab.transform.rotation);
        }
        Destroy(gameObject);
    }
}
