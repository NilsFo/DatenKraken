using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SignHover : MonoBehaviour
{
    public float speed = 0.69f;
    public float magnitude = 0.25f;
    private float dt;

    // Start is called before the first frame update
    void Start()
    {
        //Random jitter
        dt = Random.Range(0.0f, 200.0f);
    }

    // Update is called once per frame
    void Update()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        dt += Time.deltaTime * speed;
        Vector3 pos = new Vector3(x, y + MathF.Sin(dt) * magnitude, z);
        transform.position = pos;
    }
}