using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private int elementType;
    [SerializeField] private float damage;
    [SerializeField] private GameObject explosion;

    private GameManager gameManager;
    private Element element;
    private static GameObject[] cacheAllEnemyObjects = new GameObject[] {};

    public int ElementType
    {
        get { return elementType; }
    }

    public float Damage {  get { return damage; } }

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        speed = gameManager.GetEnemySpeed();

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
            OnTouchEnemy playerUnit = otherObj.GetComponent<OnTouchEnemy>();
            int playerEleType = playerUnit.elementType;
            if ( element.GetTypeAdvantage(playerEleType) != Element.TYPE_STRONGER)
            {
                TriggerExplosion();
            }
            else
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

    public static void ClearAllEnemyUnit()
    {
        cacheAllEnemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyObject in cacheAllEnemyObjects)
        {
            var enemyUnit = enemyObject.GetComponent<Enemy>();
            enemyUnit.TriggerExplosion();
        }
    }

    public static void ToggleDisplayAllEnemyUnit(bool isShow)
    {
        cacheAllEnemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        bool isDisable = false;
        float posY = 0;
        if ( !isShow )
        {
            posY = -5;
            isDisable = true;
        }
        foreach (GameObject enemyObject in cacheAllEnemyObjects)
        {
            float x = enemyObject.transform.position.x;
            float z = enemyObject.transform.position.z;
            enemyObject.transform.position = new Vector3(x, posY, z);
            enemyObject.transform.GetChild(0).gameObject.SetActive(isDisable);
        }
    }
}
