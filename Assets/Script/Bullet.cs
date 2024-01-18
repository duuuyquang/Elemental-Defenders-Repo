using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Bullet : MonoBehaviour
{
    public float speed;
    public const float DAMAGE = 10f;
    public GameObject explosion;

    private GameManager gameManager;
    private Player player;
    private SoundController soundController;

    [SerializeField] private float damage = DAMAGE;

    public float Damage
    {
        get { return damage; }
    }

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("Player").GetComponent<Player>();
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
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
            soundController.PlayBulletExplosion();
            soundController.ToggleBulletAura(false);
            if (player.PerfectChain > 0)
            {
                int score = gameManager.ConvertChainToScore(player.PerfectChain);
                gameManager.DisplayCombo(0);
                gameManager.Score += score;
                gameManager.DisplayScoreGain(transform.position, score);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            gameManager.Score += GameManager.SCORE_GAIN_TYPE_ADVANTAGE;
            gameManager.DisplayScoreGain(transform.position, GameManager.SCORE_GAIN_TYPE_ADVANTAGE);
        }
    }
}
