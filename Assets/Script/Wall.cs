using System.Collections;
using UnityEngine;

public class Wall : MonoBehaviour
{
    const float MAX_HP = 100f;

    public GameObject hpBar;
    public GameObject explositionEffect;

    [SerializeField] private float hp = MAX_HP;

    private GameManager gameManager;
    private float initialHPScale;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        initialHPScale = hpBar.transform.localScale.x;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            switch(gameManager.GameMode)
            {
                case GameManager.MODE_ATTACK:
                    ProcessEnemyExplosion(other.gameObject);
                    ProcessEnemyAttack(other.gameObject);
                    UpdateHPBar();
                    if (hp <= 0)
                    {
                        StartCoroutine(GameOver());
                    }
                    break;

                case GameManager.MODE_DEFENSE:
                    ProcessEnemyExplosion(other.gameObject);
                    StartCoroutine(GameOver());
                    break;
            }
        }
    }

    private void ProcessEnemyExplosion(GameObject enemyObj)
    {
        Instantiate(explositionEffect, gameObject.transform.position, explositionEffect.transform.rotation);
        Destroy(enemyObj);
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
        gameManager.GameOver();
        yield return new WaitForSeconds(0.5f);
        gameManager.SetGameOverMenu();
    }
}
