using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Bullet : MonoBehaviour
{
    public float speed;
    public const float DAMAGE = 10f;
    public GameObject explosion;
    public GameObject scoreGainPrefab;

    private GameManager gameManager;
    private Player player;

    [SerializeField] private float damage = DAMAGE;

    public const float PUSH_BACK_SPEED = 1f; 

    public float Damage
    {
        get { return damage; }
    }

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);

        if(transform.position.z > 50)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("EnemyWall"))
        {
            Instantiate(explosion, transform.position, explosion.transform.rotation);
            if(player.PerfectChain > 0)
            {
                int score = gameManager.ConvertChainToScore(player.PerfectChain);
                player.PerfectChain = 0;
                gameManager.Score += score;
                DisplayScoreGain(score);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            gameManager.Score += GameManager.SCORE_GAIN_TYPE_ADVANTAGE;
            DisplayScoreGain(GameManager.SCORE_GAIN_TYPE_ADVANTAGE);
        }
    }

    void DisplayScoreGain(int point)
    {
        TextMeshPro text = scoreGainPrefab.GetComponent<TextMeshPro>();
        text.text = "+" + point;
        Instantiate(scoreGainPrefab, transform.position, scoreGainPrefab.transform.rotation);
    }
}
