using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWall : MonoBehaviour
{
    const float MAX_HP = 100f;

    [SerializeField] private float hp = MAX_HP;

    private GameManager gameManager;
    private float initialHPScale;

    public GameObject explosition;
    public GameObject hpBar;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
                if (otherObj.CompareTag("Bullet"))
                {
                    ProcessExplosion(otherObj);
                    Destroy(otherObj);
                    ProcessPlayerAttack(otherObj);
                    UpdateHPBar();
                    if (hp <= 0)
                    {
                        StartCoroutine(GameOver());
                    }
                    else
                    {
                        Enemy.SetAllUnitSpeed(gameManager.GetEnemySpeed());
                    }
                }

                if (otherObj.CompareTag("Player"))
                {
                    ProcessExplosion(otherObj);
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

    IEnumerator GameOver()
    {
        Enemy.ClearAllEnemyUnit();
        gameManager.GameOver = true;
        yield return new WaitForSeconds(1);
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
}
