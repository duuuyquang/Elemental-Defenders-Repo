using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDown : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float speed = 1.0f;
    private const int MAX_SPEED = 10;
    private const int MIN_SPEED = 5;
    void Start()
    {
        speed = Random.Range(MIN_SPEED, MAX_SPEED);
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
