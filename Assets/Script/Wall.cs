using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Start is called before the first frame update
    private GameManager gameManager;
    public GameObject explositionEffect;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            StartCoroutine(GameOver(other.gameObject));
        }
    }

    IEnumerator GameOver(GameObject gameObject)
    {
        Instantiate(explositionEffect, gameObject.transform.position, explositionEffect.transform.rotation);
        Destroy(gameObject);
        gameManager.GameOver();
        yield return new WaitForSeconds(1);
        gameManager.SetGameOverMenu();
    }
}
