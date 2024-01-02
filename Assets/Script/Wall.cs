using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    // Start is called before the first frame update
    private GameManager gameManager;
    private float initialHPScale;
    public GameObject hp;
    public GameObject explositionEffect;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        initialHPScale = hp.transform.localScale.x;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            switch(gameManager.GameMode)
            {
                case GameManager.MODE_DEFENSE:
                    StartCoroutine(GameOver(other.gameObject));
                    break;
                case GameManager.MODE_ATTACK:
                    Instantiate(explositionEffect, gameObject.transform.position, explositionEffect.transform.rotation);
                    Destroy(other.gameObject);
                    gameManager.Score = (int)Mathf.Floor(gameManager.Score / 3);
                    gameManager.PlayerWallHP = Mathf.Max(gameManager.PlayerWallHP - 20, 0);
                    UpdateHPBar();
                    if (gameManager.PlayerWallHP <= 0)
                    {
                        gameManager.GameOver();
                        gameManager.SetGameOverMenu();
                    }
                    break;
            }
        }
    }

    private void UpdateHPBar()
    {
        float curPercentage = gameManager.PlayerWallHP * 0.01f;
        float newX = (initialHPScale - curPercentage) * 0.5f;
        hp.transform.localPosition = new Vector3(-newX, hp.transform.localPosition.y, hp.transform.localPosition.z);
        hp.transform.localScale = new Vector3(initialHPScale * curPercentage, hp.transform.localScale.y, hp.transform.localScale.z);
    }


    IEnumerator GameOver(GameObject gameObject)
    {
        Instantiate(explositionEffect, gameObject.transform.position, explositionEffect.transform.rotation);
        Destroy(gameObject);
        gameManager.GameOver();
        yield return new WaitForSeconds(0.5f);
        gameManager.SetGameOverMenu();
    }
}
