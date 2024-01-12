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

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player").GetComponent<Player>();
        initialHPScale = hpBar.transform.localScale.x;
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            switch(gameManager.GameMode)
            {
                case GameManager.MODE_ATTACK:
                    ProcessExplosion(other.gameObject);
                    ProcessEnemyAttack(other.gameObject);
                    UpdateHPBar();
                    gameManager.DisplayCombo(0);
                    player.UpdateGaugeBar(0);
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
        float curPercentage = hp / MAX_HP;
        float newX = (initialHPScale - curPercentage) * 0.5f;
        hpBar.transform.localPosition = new Vector3(-newX, hpBar.transform.localPosition.y, hpBar.transform.localPosition.z);
        hpBar.transform.localScale = new Vector3(initialHPScale * curPercentage, hpBar.transform.localScale.y, hpBar.transform.localScale.z);
    }

    IEnumerator GameOver()
    {
        gameManager.GameOver = true;
        yield return new WaitForSeconds(0.5f);
        gameManager.SetGameOverMenu();
    }
}
