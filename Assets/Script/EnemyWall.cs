using System.Collections;
using UnityEngine;

public class EnemyWall : MonoBehaviour
{
    const float MAX_HP = 100f;

    [SerializeField] private float hp = MAX_HP;

    private GameManager gameManager;
    private Player player;
    private float initialHPScale;

    public GameObject explosition;
    public GameObject hpBar;

    private Color defaultHPColor = Color.red;
    private Color regenHPColor = Color.yellow;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player").GetComponent<Player>();
        initialHPScale = hpBar.transform.localScale.x;
    }

    private void OnTriggerEnter(Collider other)
    {
        ProcessByGameMode(other.gameObject);
    }

    void ProcessByGameMode(GameObject otherObj)
    {
        switch (gameManager.GameMode)
        {
            case GameManager.MODE_ATTACK:
            case GameManager.MODE_ENDLESS:
                if (otherObj.CompareTag("Bullet"))
                {
                    ProcessExplosion(otherObj);
                    Destroy(otherObj);
                    ProcessPlayerAttack(otherObj);
                    UpdateHPBar();
                    if (hp <= 0)
                    {
                        StartCoroutine(GameOver());
                    } else
                    {
                        StartCoroutine(DelayAttackBeforeNextSpawn(GameManager.SEC_DELAY_AFTER_SHOOT));
                    }
                }

                if (otherObj.CompareTag("Player"))
                {
                    OnTouchEnemy player = otherObj.GetComponent<OnTouchEnemy>();
                    ProcessExplosion(otherObj);
                    gameManager.Score += GameManager.SCORE_GAIN_TYPE_HIT_WALL + player.bonusScore;
                    player.DisplayAllScoresGained(GameManager.SCORE_GAIN_TYPE_HIT_WALL);
                    Destroy(otherObj);
                }
                break;

            case GameManager.MODE_DEFENSE:
                if (otherObj.CompareTag("Player"))
                {
                    StartCoroutine(GameOver());
                }
                break;
        }
    }

    IEnumerator DelayAttackBeforeNextSpawn(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        player.IsAttacking = false;
    }

    IEnumerator GameOver()
    {
        gameManager.GameOver = true;
        yield return new WaitForSeconds(1);
        Enemy.ClearAllEnemyUnit();
        gameManager.SetWinScreen();
    }

    private void ProcessExplosion(GameObject other)
    {
        Instantiate(explosition, other.transform.position, explosition.transform.rotation);
    }

    private void UpdateHPBar()
    {
        float curPercentage = hp / MAX_HP;
        float newX = (initialHPScale - curPercentage) * 0.5f;
        hpBar.transform.localPosition = new Vector3(-newX, hpBar.transform.localPosition.y, hpBar.transform.localPosition.z);
        hpBar.transform.localScale = new Vector3(initialHPScale * curPercentage, hpBar.transform.localScale.y, hpBar.transform.localScale.z);
    }

    private void ProcessPlayerAttack(GameObject bulletObj)
    {
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        hp = Mathf.Max(hp - bullet.Damage, 0);
    }

    public void RegenHP(float regenAmount)
    {
        StartCoroutine(HpRegenEffect(regenAmount));
    }

    IEnumerator HpRegenEffect(float regenAmount)
    {
        Material hpBarColor = hpBar.GetComponent<Renderer>().material;
        hpBarColor.color = regenHPColor;
        float count = 0;
        float spf = 1.0f / 60.0f;
        float upf = regenAmount / 80;
        while (count < regenAmount)
        {
            hp = Mathf.Min(hp + upf, MAX_HP);
            UpdateHPBar();
            count += upf;
            yield return new WaitForSeconds(spf);
        }
        hpBarColor.color = defaultHPColor;
    }
}
