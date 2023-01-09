using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SignHover : MonoBehaviour
{
    public float speed = 0.69f;
    public float magnitude = 0.25f;
    private float dt;
    private float x, y, z;

    // Start is called before the first frame update
    void Start()
    {
        //Random jitter
        dt = Random.Range(0.0f, 200.0f);
        
        x = transform.position.x;
        y = transform.position.y;
        z = transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        dt += Time.deltaTime * speed;
        Vector3 pos = new Vector3(x, y + MathF.Sin(dt) * magnitude, z);
        transform.position = pos;
    }
}