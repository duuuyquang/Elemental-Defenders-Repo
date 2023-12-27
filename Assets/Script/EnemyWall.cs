using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWall : MonoBehaviour
{
    // Start is called before the first frame update
    private GameManager gameManager;
    public GameObject explositionEffect;
    public GameObject fireworkEffect;
    public GameObject enemyExplosionEffect;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(GameOver(other.gameObject));
        }
    }

    IEnumerator GameOver(GameObject gameObject)
    {
        Instantiate(explositionEffect, gameObject.transform.position, explositionEffect.transform.rotation);
        Destroy(gameObject);
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemyObject in enemyObjects)
        {
            Destroy(enemyObject);
            Instantiate(enemyExplosionEffect, enemyObject.transform.position, enemyObject.transform.rotation);
        }

        gameManager.GameOver();
        yield return new WaitForSeconds(1);
        gameManager.SetWinScreen();
        Instantiate(fireworkEffect, new Vector3(0,-6,-13) , fireworkEffect.transform.rotation);
    }
}
