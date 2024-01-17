using System.Collections;
using UnityEngine;

public class Wall : MonoBehaviour
{
    const float MAX_HP = 100f;

    public GameObject hpBar;
    public GameObject explosion;

    [SerializeField] private float hp = MAX_HP;

    private GameManager gameManager;
    private Player player;
    private float initialHPScale;
    private SoundController soundController;
    private EnemyWall enemyWall;
    private Color defaultHPColor = Color.red;
    private Color regenHPColor = Color.green;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player").GetComponent<Player>();
        initialHPScale = hpBar.transform.localScale.x;
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
        enemyWall = GameObject.Find("EnemyWall").GetComponent<EnemyWall>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            switch(gameManager.GameMode)
            {
                case GameManager.MODE_ATTACK:
                    Enemy enemy = other.gameObject.GetComponent<Enemy>();
                    ProcessExplosion(other.gameObject);
                    ProcessEnemyAttack(other.gameObject);
                    UpdateHPBar();
                    PlayHPLostEffect();
                    ToggleLowHPEffect();
                    gameManager.DisplayCombo(0);
                    player.UpdateGaugeBar(0);
                    enemyWall.RegenHP(enemy.Damage * gameManager.GetEnemyCurrentRegenRate());
                    if (hp <= 0)
                    {
                        StartCoroutine(GameOver());
                    }
                    break;
                case GameManager.MODE_DEFENSE:
                    ProcessExplosion(other.gameObject);
                    StartCoroutine(GameOver());
                    break;
            }
        }
    }

    private void ProcessExplosion(GameObject enemyObj)
    {
        Instantiate(explosion, enemyObj.transform.position, explosion.transform.rotation);
        soundController.PlayPlayerWallExplosion();
    }

    private void ProcessEnemyAttack(GameObject enemyObj)
    {
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        hp = Mathf.Max(hp - enemy.Damage, 0);
    }

    private void UpdateHPBar()
    {
        float curPercentage = Mathf.Min(1, hp / MAX_HP);
        float newX = (initialHPScale - curPercentage) * 0.5f;
        hpBar.transform.localPosition = new Vector3(-newX, hpBar.transform.localPosition.y, hpBar.transform.localPosition.z);
        hpBar.transform.localScale = new Vector3(initialHPScale * curPercentage, hpBar.transform.localScale.y, hpBar.transform.localScale.z);
    }

    private void ToggleLowHPEffect()
    {
        float curPercentage = hp / MAX_HP;
        if (curPercentage < 0.5)
        {
            StartCoroutine("LowHPEffect");
        } else
        {
            StopCoroutine("LowHPEffect");
            Material hpBarColor = hpBar.GetComponent<Renderer>().material;
            hpBarColor.color = defaultHPColor;
        }
    }

    public void RegenHP(float regenAmount)
    {
        StartCoroutine(HpRegenEffect(regenAmount));
    }

    private void PlayHPLostEffect()
    {
        StartCoroutine(HPLostEffect());
    }

    IEnumerator GameOver()
    {
        gameManager.GameOver = true;
        yield return new WaitForSeconds(0.5f);
        gameManager.SetGameOverMenu();
    }

    IEnumerator HPLostEffect()
    {
        Material hpBarColor = hpBar.GetComponent<Renderer>().material;
        int loop = 0;
        while(loop < 3)
        {
            hpBarColor.color = new Color(0, 0, 0);
            yield return new WaitForSeconds(0.1f);
            hpBarColor.color = defaultHPColor;
            yield return new WaitForSeconds(0.1f);
            loop++;
        }
    }

    IEnumerator LowHPEffect()
    {
        Material hpBarColor = hpBar.GetComponent<Renderer>().material;
        while(true)
        {
            hpBarColor.color = new Color(0, 0, 0);
            yield return new WaitForSeconds(0.2f);
            hpBarColor.color = defaultHPColor;
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator HpRegenEffect(float regenAmount)
    {
        StopCoroutine("LowHPEffect");
        Material hpBarColor = hpBar.GetComponent<Renderer>().material;
        hpBarColor.color = regenHPColor;
        soundController.PlayHealingSound();
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
        ToggleLowHPEffect();
    }
}
