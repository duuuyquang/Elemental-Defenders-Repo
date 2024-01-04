using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public float speed = 1.0f;
    [SerializeField] private int elementType;
    [SerializeField] private float damage;

    private GameManager gameManager;

    public int ElementType
    {
        get { return elementType; }
    }

    public float Damage {  get { return damage; } }

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        speed = gameManager.SetEnemySpeed();
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
