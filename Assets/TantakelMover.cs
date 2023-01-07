using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TantakelMover : MonoBehaviour {
    public Datenkrake krake;
    public Rigidbody2D endEffector;
    public Transform target;

    public float moveSpeed = 15f;

    
    // Start is called before the first frame update
    void Start() {
        //moveToPosition = endEffector.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate() {
        var posTowards = Vector2.MoveTowards(endEffector.transform.position, (Vector2)target.position, moveSpeed * Time.deltaTime);

        var delta = ((Vector2)endEffector.transform.position - (Vector2)krake.transform.position);
        var antiAttactor = delta * -1f / delta.magnitude * Time.deltaTime;
        //posTowards += antiAttactor * antiAttactor;
        endEffector.MovePosition(posTowards);

        if (((Vector2)krake.transform.position - (Vector2)target.transform.position).magnitude > 3.0f) {
            var v = krake.GetVelocity();
            if (v.magnitude < 0.01f) {
                v = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
                target.position = (Vector2)krake.transform.position + v * 2f;
                return;
            } else {
                v.Normalize();
            }
            var dir = v + Vector2.Perpendicular(v) * Random.Range(-0.5f, 0.5f);
            target.position = (Vector2)krake.transform.position + dir.normalized * 2.5f;
        }
    }
}
