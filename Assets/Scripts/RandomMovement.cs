using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovement : MonoBehaviour
{
    Vector3 random = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        random.x = Random.Range(-10.00f, 10.00f);
        random.y = Random.Range(-10.00f, 10.00f);
        random.z = Random.Range(-10.00f, 10.00f);

        transform.Rotate(random, random.x * Time.deltaTime);
        transform.Translate(random * Time.deltaTime);
    }
}
