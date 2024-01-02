using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float speed = 1.0f;

    [SerializeField]
    private int elementType;

    private GameManager gameManager;

    public int ElementType
    {
        get { return elementType; }
    }

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
}
