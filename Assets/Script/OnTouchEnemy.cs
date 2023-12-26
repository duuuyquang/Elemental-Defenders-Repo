using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class OnTouchEnemy : MonoBehaviour
{
    private Element playerElement;
    private int enemyType;

    public GameObject playerExplosionPrefab;
    public GameObject enemyExplosionPrefab;

    private void Start()
    {
    }

    private void OnCollisionEnter(Collision collision)
    {
        switch (gameObject.name)
        {
            case "Fire(Clone)":
                playerElement = new Fire();
                break;
            case "Water(Clone)":
                playerElement = new Water();
                break;
            case "Wood(Clone)":
                playerElement = new Wood();
                break;
        }

        if (collision.transform.CompareTag("Enemy"))
        {
            if (collision.gameObject.name == "EnemyFire(Clone)")
            {
                enemyType = Element.TYPE_FIRE;
            }

            if (collision.gameObject.name == "EnemyWater(Clone)")
            {
                enemyType = Element.TYPE_WATER;
            }

            if (collision.gameObject.name == "EnemyWood(Clone)")
            {
                enemyType = Element.TYPE_WOOD;
            }

            if (playerElement.GetTypeAdvantage(enemyType) == 1)
            {
                EnemyExplosive(collision.gameObject);
            }
            else if (playerElement.GetTypeAdvantage(enemyType) == 0)
            {
                PlayerExplosive();
            }
            else
            {
                Destroy(collision.gameObject);
                PlayerExplosive();
            }

        }
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
