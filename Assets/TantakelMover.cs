using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TantakelMover : MonoBehaviour {
    public Datenkrake krake;
    public Rigidbody2D endEffector;
    public Transform target;
    public bool active;

    public float moveSpeed = 15f;

    private float _moveTimer;
    private float _waveTimer;
    private float _waveInterval = 4f;
    public float moveOffset;
    public float moveInterval = 1f;

    public Joint2D[] joints;

    
    // Start is called before the first frame update
    void Start() {
        //moveToPosition = endEffector.transform.position;
        _moveTimer = moveOffset;
        _waveTimer = moveOffset / moveInterval * _waveInterval;
    }

    void Update() {
        _waveTimer += Time.deltaTime;
        if (_waveTimer > _waveInterval)
            _waveTimer -= _waveInterval;
        for (var index = 0; index < joints.Length; index++) {
            var joint = joints [index];
            var alpha = (_waveTimer / _waveInterval ) * Mathf.PI * 2 + ((float)index / (float)joints.Length) * Mathf.PI * 2;
            joint.attachedRigidbody.AddRelativeForce(new Vector2(Mathf.Sin(alpha), -Mathf.Cos(alpha)) * (200f * Time.deltaTime));
        }
    }

    // Update is called once per frame
    void FixedUpdate() {
        if (!active)
            return;
        var posTowards = Vector2.MoveTowards(endEffector.transform.position, (Vector2)target.position, (moveSpeed + krake.GetVelocity().magnitude) * Time.deltaTime);

        var delta = ((Vector2)endEffector.transform.position - (Vector2)krake.transform.position);
        var antiAttactor = delta * -1f / delta.magnitude * Time.deltaTime;
        posTowards += antiAttactor * antiAttactor;
        endEffector.MovePosition(posTowards);

        var v = krake.GetVelocity();
        _moveTimer += Time.deltaTime;
        if (_moveTimer > moveInterval) {
            _moveTimer -= moveInterval;
            if (((Vector2)krake.transform.position - ((Vector2)target.transform.position - v.normalized * 1f)).magnitude > 2.5f) {
                if (v.magnitude < 0.01f) {
                    target.position = (Vector2)endEffector.transform.position;
                    return;
                } else {
                    v.Normalize();
                }
                var dir = v + Vector2.Perpendicular(v) * Random.Range(-1f, 1f);
                var pos = (Vector2)krake.transform.position + dir.normalized * Random.Range(2.5f, 5.5f);
                pos = krake.currentAdBox.ClosestPoint(pos);
                target.position = pos;
                target.parent = krake.currentAdBox.transform;
            }
        }
    }
}
