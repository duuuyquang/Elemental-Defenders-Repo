using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private int elementType;
    [SerializeField] private float damage;
    [SerializeField] private GameObject explosion;

    private GameManager gameManager;
    private Element element;

    public int ElementType
    {
        get { return elementType; }
    }

    public float Damage {  get { return damage; } }

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        speed = gameManager.SetEnemySpeed();

        switch (elementType)
        {
            case Element.TYPE_FIRE:
                element = new Fire();
                break;
            case Element.TYPE_WATER:
                element = new Water();
                break;
            case Element.TYPE_WOOD:
                element = new Wood();
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {   
        transform.Translate(Vector3.back * Time.deltaTime * speed);

        if(transform.position.z < -90)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject otherObj = collision.gameObject;
        if(otherObj.CompareTag("Player"))
        {
            var playerUnit = otherObj.GetComponent<OnTouchEnemy>();
            if (element.GetTypeAdvantage(playerUnit.elementType) != Element.TYPE_STRONGER)
            {
                TriggerExplosion();
            } else
            {
                speed = 13f; // to rush end the wave
            }
        }

        if(otherObj.CompareTag("Bullet"))
        {
            TriggerExplosion();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PlayerWall"))
        {
            TriggerExplosion();
        }
    }

    public void TriggerExplosion()
    {
        Destroy(gameObject);
        Instantiate(explosion, transform.position, explosion.transform.rotation);
    }

    public static void SetAllUnitSpeed(float speed)
    {
        GameObject[] enemyObjList = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemyObjList.Length > 0)
        {
            foreach (GameObject enemyObj in enemyObjList)
            {
                Enemy enemy = enemyObj.GetComponent<Enemy>();
                enemy.speed = speed;
            }
        }
    }
}
